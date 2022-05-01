namespace Solidarity.Application.Common;

public interface IDatabase
{
	DbSet<Account> Accounts { get; set; }
	DbSet<Identity> Identities { get; set; }
	DbSet<AuthenticationMethod> AuthenticationMethods { get; set; }
	DbSet<Handshake> Handshakes { get; set; }
	DbSet<Campaign> Campaigns { get; set; }
	DbSet<Validation> Validations { get; set; }
	DbSet<Vote> Votes { get; set; }
	DbSet<DonationChannel> DonationChannels { get; set; }
	DbSet<PaymentMethodKey> PaymentMethodKeys { get; set; }
	DbSet<CampaignMedia> CampaignMedia { get; set; }
	DbSet<CampaignExpenditure> CampaignExpenditures { get; set; }

	DbSet<TEntity> GetSet<TEntity>() where TEntity : class;
	EntityEntry GetEntry(object entity);
	void CommitChanges();
	// Task SaveChangesAsync();
}