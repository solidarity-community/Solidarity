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

	public async Task<decimal> GetBalance(int id, int? accountId = null)
	{
		var campaign = await Get(id);
		var account = accountId is null ? null : await _accountService.Get((int)accountId);
		var balances = await Task.WhenAll(
			campaign.ActivatedPaymentMethods.Select(pm => _paymentMethodProvider.Get(pm.Identifier).GetBalance(campaign, account))
		);
		return balances?.Sum() ?? 0;
	}

	public Task<decimal> GetShare(int id)
		=> _currentUserService.Id is null ? Task.FromResult(0m) : GetBalance(id, _currentUserService.Id);

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
		campaign.AllocationDate = null;
		campaign.CompletionDate = null;
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

	public async Task DeclareAllocationPhase(int campaignId)
	{
		var campaign = await Get(campaignId);

		if (_currentUserService.Id != campaign.CreatorId)
		{
			throw new Exception("You are not allowed to declare allocation phase of this campaign.");
		}

		if (campaign.Status != CampaignStatus.Funding)
		{
			throw new Exception("The campaign is not in the funding phase.");
		}

		campaign.TargetAllocationDate = DateTime.Now;
		campaign.AllocationDate = DateTime.Now;
		await _database.CommitChangesAsync();
	}

	public async Task Allocate(int campaignId, Dictionary<string, string> destinationByPaymentMethodIdentifier)
	{
		var campaign = await Get(campaignId);

		if (_currentUserService.Id != campaign.CreatorId)
		{
			throw new Exception("You are not allowed to allocate the funds of this campaign");
		}

		if (Enumerable.SequenceEqual(campaign.ActivatedPaymentMethods.Select(p => p.Identifier), destinationByPaymentMethodIdentifier.Keys) == false)
		{
			throw new ArgumentException("The number of payment methods does not match the number of destinations",
				nameof(destinationByPaymentMethodIdentifier));
		}

		if (campaign.Status != CampaignStatus.Allocation)
		{
			throw new Exception("The campaign is not in the allocation phase.");
		}

		Task AllocatePaymentMethod(CampaignPaymentMethod p) =>
			_paymentMethodService.Get(p.Identifier).Allocate(campaign, destinationByPaymentMethodIdentifier.TryGet(p.Identifier));

		await Task.WhenAll(campaign.ActivatedPaymentMethods.Select(p => AllocatePaymentMethod(p)));
		campaign.CompletionDate = DateTime.Now;
		await _database.CommitChangesAsync();
	}

	public override async Task<Campaign> Delete(int id)
	{
		await Refund(id);
		return await base.Delete(id);
	}

	private async Task Refund(int campaignId)
	{
		var campaign = await Get(campaignId);
		await Task.WhenAll(campaign.ActivatedPaymentMethods.Select(p => _paymentMethodService.Get(p.Identifier).Refund(campaign, null)));
	}
}