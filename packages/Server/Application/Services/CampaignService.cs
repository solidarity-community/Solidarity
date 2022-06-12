namespace Solidarity.Application.Services;

public class CampaignService : CrudService<Campaign>
{
	private readonly PaymentMethodService _paymentMethodService;
	private readonly AccountService _accountService;

	public CampaignService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, PaymentMethodService paymentMethodService, AccountService accountService) : base(database, paymentMethodProvider, currentUserService)
	{
		_paymentMethodService = paymentMethodService;
		_accountService = accountService;
	}

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
		var balances = await Task.WhenAll(
			campaign.ActivatedPaymentMethods.Select(pm => _paymentMethodProvider.Get(pm.Identifier).GetBalance(campaign, null))
		);
		return balances?.Sum() ?? 0;
	}

	public async Task<Dictionary<string, string>> GetDonationData(int campaignId)
	{
		var campaign = await Get(campaignId);
		var account = _currentUserService.Id is null ? null : await _accountService.Get((int)_currentUserService.Id);
		var activatedPaymentMethods = _paymentMethodService.GetAll().Where(pm => campaign.ActivatedPaymentMethods.Any(apm => apm.Identifier == pm.Identifier)).ToList();
		var data = await Task.WhenAll(activatedPaymentMethods.Select(pm => pm.GetDonationData(campaign, account)));
		return data.Select((d, i) => new { d, i = activatedPaymentMethods[i].Identifier }).ToDictionary(d => d.i, d => d.d);
	}

	public override async Task<Campaign> Create(Campaign campaign)
	{
		if (campaign.TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}
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

		if (campaign.TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}

		entity.Media = campaign.Media;
		entity.Expenditures = campaign.Expenditures;
		entity.ActivatedPaymentMethods = campaign.ActivatedPaymentMethods.Any()
			? campaign.ActivatedPaymentMethods
			: _paymentMethodService
				.GetAll()
				.Select(pm => new CampaignPaymentMethod
				{
					Identifier = pm.Identifier,
					CampaignId = entity.Id
				}).ToList();

		return (await base.Update(campaign)).WithoutAuthenticationData();
	}
}