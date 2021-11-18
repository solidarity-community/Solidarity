namespace Solidarity.Application.Services;

public class CampaignService : CrudService<Campaign>
{
	public CampaignService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService) { }

	public override Campaign Get(int id)
	{
		return base.Get(id).WithoutAuthenticationData();
	}

	public override IEnumerable<Campaign> GetAll()
	{
		return base.GetAll().WithoutAuthenticationData();
	}

	public decimal GetBalance(int id)
	{
		var campaign = Get(id);
		var client = cryptoClientFactory.GetClient(CoinType.Bitcoin, NetworkType.TestNet);
		var balance = campaign.DonationChannels
			.Select(dc => client.GetBalance(dc.WalletAddress))
			.Sum();
		return balance.ToDecimal(MoneyUnit.Satoshi);
	}

	public override Campaign Create(Campaign campaign)
	{
		campaign.Completion = null;

		base.Create(campaign);

		var cryptoClient = cryptoClientFactory.GetClient(CoinType.Bitcoin, NetworkType.TestNet);
		var address = cryptoClient.GetAddress(cryptoClient.DeriveKey(cryptoPrivateKey, campaign.Id));
		var addressText = address.ToString();
		if (addressText == null)
		{
			throw new Exception("Could not generate a crypto address for this campaign");
		}

		campaign.DonationChannels = new List<DonationChannel> {
				new DonationChannel {
					Campaign = campaign,
					WalletAddress = addressText,
					// This is the public wallet address
					Donor = null,
				}
			};

		database.CommitChanges();

		return campaign.WithoutAuthenticationData();
	}

	public override Campaign Update(Campaign campaign)
	{
		if (Get(campaign.Id).CreatorId != currentUserService.Id)
		{
			throw new Exception("You are not allowed to edit this campaign");
		}
		base.Update(campaign);
		return campaign.WithoutAuthenticationData();
	}
}