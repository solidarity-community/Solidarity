namespace Solidarity.Application.Common;

public interface IDatabase
{
	/// <summary>
	/// This method is called by the application to initialize the database.
	/// Tasks such as seeding data and applying migrations should be done here.
	/// </summary>
	void Initialize();

	DbSet<Audit> Audits { get; set; }
	DbSet<Account> Accounts { get; set; }
	DbSet<AccountRecoveryHandshake> AccountRecoveryHandshakes { get; set; }
	DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; }
	DbSet<Campaign> Campaigns { get; set; }
	DbSet<CampaignMedia> CampaignMedia { get; set; }

	ChangeTracker ChangeTracker { get; }

	DbSet<TEntity> GetSet<TEntity>() where TEntity : class;
	EntityEntry GetEntry(object entity);
	Task<int> CommitChanges(CancellationToken? cancellationToken = default);
	Task<int> CommitChangesAsSystem(CancellationToken? cancellationToken = default);
}