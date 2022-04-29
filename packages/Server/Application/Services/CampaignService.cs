namespace Solidarity.Application.Services;

public class CampaignService : CrudService<Campaign>
{
	public CampaignService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public override Campaign Get(int id)
	{
		return base.Get(id).WithoutAuthenticationData();
	}

	public override IEnumerable<Campaign> GetAll()
	{
		return base.GetAll().WithoutAuthenticationData();
	}

	public async Task<decimal> GetBalance(int id)
	{
		var campaign = Get(id);
		var paymentChannels = campaign.DonationChannels
			.Select(dc => _paymentMethodProvider.Get(dc.PaymentMethodIdentifier).GetChannel(dc.Id));
		var balances = await Task.WhenAll(paymentChannels.Select(c => c.GetBalance()));
		return balances.Sum();
	}

	public override Campaign Create(Campaign campaign)
	{
		if (campaign.TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}

		campaign.Completion = null;
		campaign.DonationChannels = _paymentMethodProvider
			.GetAll()
			.Select(pm => new DonationChannel { PaymentMethodIdentifier = pm.Identifier })
			.ToList();
		base.Create(campaign);
		return campaign.WithoutAuthenticationData();
	}

	public override Campaign Update(Campaign campaign)
	{
		var entity = Get(campaign.Id);

		if (entity.CreatorId != _currentUserService.Id)
		{
			throw new Exception("You are not allowed to edit this campaign");
		}

		if (campaign.TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}

		entity.Media = campaign.Media;
		entity.Expenditures = campaign.Expenditures;
		return base.Update(campaign).WithoutAuthenticationData();
	}
}