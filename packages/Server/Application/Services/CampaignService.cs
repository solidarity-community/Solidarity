namespace Solidarity.Application.Services;

public class CampaignService : CrudService<Campaign>
{
	public CampaignService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public override async Task<Campaign> Get(int id)
	{
		return (await base.Get(id)).WithoutAuthenticationData();
	}

	public override async Task<IEnumerable<Campaign>> GetAll()
	{
		return (await base.GetAll()).WithoutAuthenticationData();
	}

	public async Task<decimal> GetBalance(int id)
	{
		var campaign = await Get(id);
		var paymentChannels = campaign.DonationChannels
			.Select(dc => _paymentMethodProvider.Get(dc.PaymentMethodIdentifier).GetChannel(id)).ToList();
		var balances = await Task.WhenAll(paymentChannels.Select(c => c.GetBalance()));
		return balances.Sum();
	}

	public override async Task<Campaign> Create(Campaign campaign)
	{
		ValidateCampaign(campaign);
		campaign.Completion = null;
		await base.Create(campaign);
		return campaign.WithoutAuthenticationData();
	}

	public override async Task<Campaign> Update(Campaign campaign)
	{
		var entity = await Get(campaign.Id);

		if (entity.CreatorId != _currentUserService.Id)
		{
			throw new Exception("You are not allowed to edit this campaign");
		}

		ValidateCampaign(campaign);

		entity.Media = campaign.Media;
		entity.Expenditures = campaign.Expenditures;
		entity.DonationChannels = campaign.DonationChannels;
		return (await base.Update(campaign)).WithoutAuthenticationData();
	}

	private void ValidateCampaign(Campaign campaign)
	{
		if (campaign.TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}

		campaign.DonationChannels.Select(dc => _paymentMethodProvider.Get(dc.PaymentMethodIdentifier))
			.Distinct()
			.ToList()
			.ForEach(pm => pm.EnsureChannelCreated(campaign.Id));
	}
}