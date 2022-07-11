namespace Solidarity.Application.Campaigns;

[TransientService]
public class CampaignService : CrudService<Campaign>
{
	private readonly PaymentMethodService _paymentMethodService;
	private readonly AccountService _accountService;

	public CampaignService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, PaymentMethodService paymentMethodService, AccountService accountService) : base(database, paymentMethodProvider, currentUserService)
	{
		_paymentMethodService = paymentMethodService;
		_accountService = accountService;
	}

	private async Task<Campaign> GetAndEnsureCreatedByCurrentUser(int id)
	{
		var campaign = await Get(id);
		return campaign.CreatorId != _currentUserService.Id
			? throw new InvalidOperationException("Operation is only allowed for the campaign creator.")
			: campaign;
	}

	public async Task<decimal> GetBalance(int id, int? accountId = null)
	{
		var campaign = await Get(id);
		var account = accountId.HasValue is false ? null : await _accountService.Get(accountId.Value);
		var balances = await Task.WhenAll(
			campaign.ActivatedPaymentMethods.Select(pm => _paymentMethodProvider.Get(pm.Identifier).GetChannel(campaign).GetBalance(account))
		);
		return balances?.Sum() ?? 0;
	}

	public Task<decimal> GetShare(int id)
		=> _currentUserService.Id is null ? Task.FromResult(0m) : GetBalance(id, _currentUserService.Id);

	public async Task<Dictionary<string, string>> GetDonationData(int campaignId)
	{
		var campaign = await Get(campaignId);
		var account = await _accountService.Get();
		var activatedPaymentMethods = _paymentMethodService.GetAll().Where(pm => campaign.ActivatedPaymentMethods.Any(apm => apm.Identifier == pm.Identifier)).ToList();
		var data = await Task.WhenAll(activatedPaymentMethods.Select(pm => pm.GetChannel(campaign).GetDonationData(account)));
		return data.Select((d, i) => new { d, i = activatedPaymentMethods[i].Identifier }).ToDictionary(d => d.i, d => d.d);
	}

	public override async Task<Campaign> Create(Campaign campaign)
	{
		campaign.ValidateForCreation();
		await base.Create(campaign);
		return campaign;
	}

	public override async Task<Campaign> Update(Campaign campaign)
	{
		var entity = await GetAndEnsureCreatedByCurrentUser(campaign.Id);

		entity.ValidateForUpdate(campaign);

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

		return await base.Update(campaign);
	}

	public async Task InitiateValidation(int campaignId)
	{
		var campaign = await GetAndEnsureCreatedByCurrentUser(campaignId);

		var balance = await GetBalance(campaignId);
		campaign.TransitionToValidationPhase(balance);

		await _database.CommitChangesAsync();
	}

	public async Task Allocate(int campaignId)
	{
		var campaign = await GetAndEnsureCreatedByCurrentUser(campaignId);
		campaign.EnsureNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);

		var votesStatus = await GetVotes(campaignId);

		if (votesStatus.EndorsedBalance / votesStatus.Balance < (decimal)votesStatus.ApprovalThreshold)
		{
			await campaign.Refund(p => _paymentMethodService.Get(p.Identifier).GetChannel(campaign).RefundRemaining().Allocate());
		}
		else
		{
			var accountsToRefund = campaign.Validation!.Votes
				.Where(vote => vote.Value is false)
				.Select(vote => vote.Account)
				.ToList();

			await campaign.Allocate(p => _paymentMethodService.Get(p.Identifier).GetChannel(campaign).RefundRange(accountsToRefund).FundRemaining().Allocate());
			await _database.CommitChangesAsync();
		}

	}

	public async Task<bool?> GetVote(int campaignId)
	{
		var campaign = await Get(campaignId);
		return campaign.Validation?.Votes.Find(v => v.AccountId == _currentUserService.Id)?.Value;
	}

	public async Task<CampaignVotes> GetVotes(int id)
	{
		var campaign = await Get(id);
		campaign.EnsureNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);
		var balance = await GetBalance(id);
		var endorsedDonations = 0m;
		foreach (var vote in campaign.Validation!.Votes.Where(vote => vote.Value == true))
		{
			endorsedDonations += await GetBalance(id, vote.AccountId);
		}
		return new(endorsedDonations, balance, campaign.Validation!.ApprovalThreshold);
	}

	public async Task Vote(int campaignId, bool vote)
	{
		var campaign = await Get(campaignId);
		campaign.Vote(_currentUserService.Id!.Value, vote);
		await _database.CommitChangesAsync();
	}

	public override async Task<Campaign> Delete(int id)
	{
		var campaign = await Get(id);
		await campaign.Refund(p => _paymentMethodService.Get(p.Identifier).GetChannel(campaign).RefundRemaining().Allocate());
		return await base.Delete(id);
	}
}