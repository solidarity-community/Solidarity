using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using static Solidarity.Application.EnvironmentVariables;

namespace Solidarity.Infrastructure.Persistence;

public sealed class DatabaseContext : DbContext, IDatabase
{
	public static readonly string connectionString = DATABASE_SERVER is null || DATABASE_USER is null || DATABASE_PASSWORD is null
		? "Data Source=localhost\\SQLEXPRESS;Database=Solidarity;Integrated Security=True;Encrypt=False;"
		: $"Server={DATABASE_SERVER};User ID={DATABASE_USER};Database=Solidarity;Password={DATABASE_PASSWORD};Encrypt=False;MultipleActiveResultSets=True;";

	/// <summary>
	/// The target migration to migrate to. If null, the latest migration will be used.
	/// </summary>
	private const string? TargetMigration = null;

	private readonly ILogger logger;
	private readonly IAuthenticatedAccount authenticatedAccount;
	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		=> (logger, authenticatedAccount) = (this.GetService<ILogger<DatabaseContext>>(), this.GetService<IAuthenticatedAccount>());

	public void Initialize()
	{
		logger.LogInformation("Initializing database with the provider name {providerName}", Database.ProviderName);

		// Uncomment to drop the entire database.
		// Database.EnsureDeleted();

		if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
		{
			logger.LogInformation("Migrating database to {migration}.", TargetMigration ?? "the latest migration");
			Database.GetInfrastructure().GetRequiredService<IMigrator>().Migrate(TargetMigration);
		}

		// await EnsureSystemAccountSeeded();
		// await EnsureAdminAccountSeeded();
		// AuthenticatedAccount.systemAccountId.Value = Accounts.Single(a => a.Role == AccountRole.System).Id;
	}

	public DbSet<Audit> Audits { get; set; } = null!;
	public DbSet<Account> Accounts { get; set; } = null!;
	public DbSet<AccountRecoveryHandshake> AccountRecoveryHandshakes { get; set; } = null!;
	public DbSet<Campaign> Campaigns { get; set; } = null!;
	public DbSet<CampaignMedia> CampaignMedia { get; set; } = null!;
	public DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; } = null!;

	public DbSet<TEntity> GetSet<TEntity>() where TEntity : class => Set<TEntity>();
	public EntityEntry GetEntry(object entity) => Entry(entity);

	public Task<int> CommitChanges(CancellationToken? cancellationToken = default) => cancellationToken.HasValue ? SaveChangesAsync(cancellationToken.Value) : SaveChangesAsync();
	public async Task<int> CommitChangesAsSystem(CancellationToken? cancellationToken = default)
	{
		using (authenticatedAccount.System)
		{
			return await CommitChanges(cancellationToken);
		}
	}

	protected override void OnModelCreating(ModelBuilder b)
	{
		Account.ConfigureDatabase(b);
		Campaign.ConfigureDatabase(b);
		CampaignValidation.ConfigureDatabase(b);
		CampaignAllocation.ConfigureDatabase(b);
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		configurationBuilder.Conventions.Add(serviceProvider => new AutoIncludeAttributeConvention(serviceProvider.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
	}
}