namespace Solidarity.Application.Abstractions;

public interface IDatabase
{
	/// <summary>
	/// This method is called by the application to initialize the database.
	/// Tasks such as seeding data and applying migrations should be done here.
	/// </summary>
	void Initialize();

	DbSet<Account> Accounts { get; set; }
	DbSet<AccountProfile> AccountProfiles { get; set; }
	DbSet<AuthenticationMethod> AuthenticationMethods { get; set; }
	DbSet<AccountRecoveryHandshake> AccountRecoveryHandshakes { get; set; }
	DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; }
	DbSet<Campaign> Campaigns { get; set; }
	DbSet<CampaignValidation> CampaignValidations { get; set; }
	DbSet<CampaignValidationVote> CampaignValidationVotes { get; set; }
	DbSet<CampaignPaymentMethod> CampaignPaymentMethods { get; set; }
	DbSet<CampaignMedia> CampaignMedia { get; set; }
	DbSet<CampaignExpenditure> CampaignExpenditures { get; set; }

	DbSet<TEntity> GetSet<TEntity>() where TEntity : class;
	EntityEntry GetEntry(object entity);
	void CommitChanges();
	Task CommitChangesAsync();
}