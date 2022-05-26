namespace Solidarity.Infrastructure.Persistence;

public class DatabaseContext : DbContext, IDatabase
{
	private readonly ICurrentUserService _currentUserService;

	public DatabaseContext(DbContextOptions<DatabaseContext> options, ICurrentUserService currentUserService) : base(options) => _currentUserService = currentUserService;

	public void Initialize()
	{
		if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
		{
			Database.Migrate();
		}
	}

	public DbSet<Account> Accounts { get; set; } = null!;
	public DbSet<Domain.Models.Identity> Identities { get; set; } = null!;
	public DbSet<AuthenticationMethod> AuthenticationMethods { get; set; } = null!;
	public DbSet<Handshake> Handshakes { get; set; } = null!;
	public DbSet<Campaign> Campaigns { get; set; } = null!;
	public DbSet<CampaignMedia> CampaignMedia { get; set; } = null!;
	public DbSet<CampaignDonationChannel> CampaignDonationChannels { get; set; } = null!;
	public DbSet<CampaignExpenditure> CampaignExpenditures { get; set; } = null!;
	public DbSet<Validation> Validations { get; set; } = null!;
	public DbSet<Vote> Votes { get; set; } = null!;
	public DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; } = null!;

	public DbSet<TEntity> GetSet<TEntity>() where TEntity : class => Set<TEntity>();
	public EntityEntry GetEntry(object entity) => Entry(entity);

	public void CommitChanges() => SaveChanges();
	public Task CommitChangesAsync() => SaveChangesAsync();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Account>(a => a.HasIndex(e => e.Username).IsUnique());
		modelBuilder.Entity<Account>().HasMany(a => a.Campaigns).WithOne(c => c.Creator);
		modelBuilder.Entity<Account>().HasMany(a => a.Votes).WithOne(v => v.Account);

		modelBuilder.Entity<Domain.Models.Identity>().HasOne(i => i.Account).WithOne();

		modelBuilder.Entity<Handshake>().HasOne(h => h.Account);

		modelBuilder.Entity<AuthenticationMethod>().Ignore(am => am.SupportsMultiple);
		modelBuilder.Entity<AuthenticationMethod>().HasKey(am => new { am.AccountId, am.Type, am.Salt });
		modelBuilder.Entity<AuthenticationMethod>().HasDiscriminator(am => am.Type)
			.HasValue<PasswordAuthentication>(AuthenticationMethodType.Password);

		modelBuilder.Entity<Campaign>().HasOne(c => c.Validation).WithOne(v => v.Campaign);
		modelBuilder.Entity<Campaign>().HasMany(c => c.Media).WithOne();
		modelBuilder.Entity<Campaign>().HasMany(c => c.Expenditures).WithOne();
		modelBuilder.Entity<Campaign>().HasMany(c => c.DonationChannels).WithOne(dc => dc.Campaign);

		modelBuilder.Entity<PaymentMethodKey>().HasKey(pmk => pmk.PaymentMethodIdentifier);

		modelBuilder.Entity<Validation>().HasMany(v => v.Votes).WithOne(v => v.Validation);
	}

	public override int SaveChanges()
	{
		HandleSaveChanges();
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		HandleSaveChanges();
		return base.SaveChangesAsync(cancellationToken);
	}

	private void HandleSaveChanges()
	{
		var userId = _currentUserService.Id;
		foreach (var entry in ChangeTracker.Entries<Model>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatorId = userId;
					entry.Entity.Creation = DateTime.Now;
					break;
				case EntityState.Modified:
					entry.Entity.LastModifierId = userId;
					entry.Entity.LastModification = DateTime.Now;
					break;
			}
		}
	}
}