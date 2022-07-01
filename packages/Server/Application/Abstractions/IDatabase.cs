namespace Solidarity.Application.Abstractions;

public interface IDatabase
{
	/// <summary>
	/// This method is called by the application to initialize the database.
	/// Tasks such as seeding data and applying migrations should be done here.
	/// </summary>
	void Initialize();

	DbSet<Account> Accounts { get; set; }
	DbSet<Identity> Identities { get; set; }
	DbSet<AuthenticationMethod> AuthenticationMethods { get; set; }
	DbSet<Handshake> Handshakes { get; set; }
	DbSet<Campaign> Campaigns { get; set; }
	DbSet<Validation> Validations { get; set; }
	DbSet<Vote> Votes { get; set; }
	DbSet<CampaignPaymentMethod> CampaignPaymentMethods { get; set; }
	DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; }
	DbSet<CampaignMedia> CampaignMedia { get; set; }
	DbSet<CampaignExpenditure> CampaignExpenditures { get; set; }

	DbSet<TEntity> GetSet<TEntity>() where TEntity : class;
	EntityEntry GetEntry(object entity);
	void CommitChanges();
	Task CommitChangesAsync();
}