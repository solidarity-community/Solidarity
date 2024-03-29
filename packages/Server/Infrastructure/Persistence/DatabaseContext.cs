﻿namespace Solidarity.Infrastructure.Persistence;

public class DatabaseContext : DbContext, IDatabase
{
	private readonly ICurrentUserService _currentUserService;
	private readonly ILogger _logger;

	public DatabaseContext(DbContextOptions<DatabaseContext> options, ICurrentUserService currentUserService, ILogger<DatabaseContext> logger) : base(options)
	{
		_currentUserService = currentUserService;
		_logger = logger;
	}

	public void Initialize()
	{
		_logger.LogInformation(@"Initializing database with the provider name ""{providerName}""", Database.ProviderName);

		var reset = false;
		if (reset)
		{
			Database.EnsureDeleted();
		}

		Database.EnsureCreated();

		if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
		{
			_logger.LogInformation("Migrating database");
			Database.Migrate();
		}
	}

	public DbSet<Account> Accounts { get; set; } = null!;
	public DbSet<AccountProfile> AccountProfiles { get; set; } = null!;
	public DbSet<AuthenticationMethod> AuthenticationMethods { get; set; } = null!;
	public DbSet<AccountRecoveryHandshake> AccountRecoveryHandshakes { get; set; } = null!;
	public DbSet<Campaign> Campaigns { get; set; } = null!;
	public DbSet<CampaignMedia> CampaignMedia { get; set; } = null!;
	public DbSet<CampaignExpenditure> CampaignExpenditures { get; set; } = null!;
	public DbSet<CampaignValidation> CampaignValidations { get; set; } = null!;
	public DbSet<CampaignValidationVote> CampaignValidationVotes { get; set; } = null!;
	public DbSet<CampaignPaymentMethod> CampaignPaymentMethods { get; set; } = null!;
	public DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; } = null!;

	public DbSet<TEntity> GetSet<TEntity>() where TEntity : class => Set<TEntity>();
	public EntityEntry GetEntry(object entity) => Entry(entity);

	public void CommitChanges() => SaveChanges();
	public Task CommitChangesAsync() => SaveChangesAsync();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Account>(a => a.HasIndex(e => e.Username).IsUnique());
		modelBuilder.Entity<Account>().HasMany(a => a.Campaigns).WithOne().HasForeignKey(c => c.CreatorId);
		modelBuilder.Entity<Account>().HasMany(a => a.Votes).WithOne(v => v.Account);

		modelBuilder.Entity<AccountProfile>().HasOne(i => i.Account).WithOne();

		modelBuilder.Entity<AccountRecoveryHandshake>().HasOne(h => h.Account);

		modelBuilder.Entity<AuthenticationMethod>().Ignore(am => am.SupportsMultiple);
		modelBuilder.Entity<AuthenticationMethod>().HasKey(am => new { am.AccountId, am.Type, am.Salt });
		modelBuilder.Entity<AuthenticationMethod>().HasDiscriminator(am => am.Type)
			.HasValue<PasswordAuthenticationMethod>(AuthenticationMethodType.Password);

		modelBuilder.Entity<Campaign>().HasOne(c => c.Validation).WithOne(v => v.Campaign);
		modelBuilder.Entity<Campaign>().HasOne(c => c.Allocation).WithOne(a => a.Campaign);
		modelBuilder.Entity<Campaign>().HasMany(c => c.Media).WithOne();
		modelBuilder.Entity<Campaign>().HasMany(c => c.Expenditures).WithOne();
		modelBuilder.Entity<Campaign>().HasMany(c => c.ActivatedPaymentMethods).WithOne(pm => pm.Campaign);

		modelBuilder.Entity<PaymentMethodKey>().HasKey(pmk => pmk.PaymentMethodIdentifier);

		modelBuilder.Entity<CampaignValidation>().HasMany(v => v.Votes).WithOne(v => v.Validation);

		modelBuilder.Entity<CampaignValidationVote>().Ignore(v => v.Id);
		modelBuilder.Entity<CampaignValidationVote>().HasKey(v => new { v.ValidationId, v.AccountId });

		modelBuilder.Entity<CampaignAllocation>().HasMany(a => a.Entries).WithOne(e => e.CampaignAllocation);
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