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
		var paymentChannels = await Task.WhenAll(
			campaign.DonationChannels.Select(
				dc => _paymentMethodProvider.Get(dc.PaymentMethodIdentifier).GetChannel(id)
			)
		);
		var balances = await Task.WhenAll(paymentChannels.Select(c => c.GetBalance()));
		return balances.Sum();
	}

	public override async Task<Campaign> Create(Campaign campaign)
	{
		await ValidateCampaign(campaign);
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

		await ValidateCampaign(campaign);

		entity.Media = campaign.Media;
		entity.Expenditures = campaign.Expenditures;
		entity.DonationChannels = campaign.DonationChannels;
		return (await base.Update(campaign)).WithoutAuthenticationData();
	}

	private async Task ValidateCampaign(Campaign campaign)
	{
		if (campaign.TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}

		await Task.WhenAll(
			campaign.DonationChannels.Select(dc => _paymentMethodProvider.Get(dc.PaymentMethodIdentifier))
				.Distinct()
				.ToList()
				.Select(pm => pm.EnsureChannelCreated(campaign.Id))
		);
	}
}