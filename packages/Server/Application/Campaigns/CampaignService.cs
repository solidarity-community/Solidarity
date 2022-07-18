namespace Solidarity.Application.Campaigns;

[TransientService]
public class CampaignService : CrudService<Campaign>
{
	private readonly AccountService _accountService;
	public CampaignService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, AccountService accountService) : base(database, paymentMethodProvider, currentUserService)
		=> _accountService = accountService;

	private async Task<Campaign> GetAndEnsureCreatedByCurrentUser(int id)
	{
		var campaign = await Get(id);
		return campaign.CreatorId != _currentUserService.Id
			? throw new InvalidOperationException("Operation is only allowed for the campaign creator.")
			: campaign;
	}

	public async Task<double> GetBalance(int id, int? accountId = null)
	{
		var campaign = await Get(id);
		var account = accountId.HasValue is false ? null : await _accountService.Get(accountId.Value);
		return await campaign.GetBalance(_paymentMethodProvider, account);
	}

	public async Task<double> GetTotalBalance(int id)
	{
		var campaign = await Get(id);
		return await campaign.GetTotalBalance(_paymentMethodProvider);
	}

	public Task<double> GetShare(int id) => GetBalance(id, _currentUserService.Id);

	public async Task<Dictionary<string, string>> GetDonationData(int campaignId)
	{
		var campaign = await Get(campaignId);
		var activatedPaymentMethods = campaign.ActivatedPaymentMethods.Select(pm => _paymentMethodProvider.Get(pm.Identifier)).ToList();
		var account = await _accountService.Get();

		var totalDonations = await campaign.GetTotalBalance(_paymentMethodProvider);
		var publicDonations = await campaign.GetBalance(_paymentMethodProvider, null);
		var privateDonations = totalDonations - publicDonations;

		if (account is null && privateDonations / campaign.TotalExpenditure <= CampaignValidation.ApprovalThresholdPercentage)
		{
			throw new PublicDonationsLockedException();
		}

		var data = await Task.WhenAll(activatedPaymentMethods.Select(pm => pm.GetChannel(campaign).GetDonationData(account)));
		return data
			.Select((d, i) => new { d, i = activatedPaymentMethods[i].Identifier })
			.ToDictionary(d => d.i, d => d.d);
	}

	public override async Task<Campaign> Create(Campaign campaign)
	{
		ValidateAllocationDestinations(campaign);
		campaign.ValidateForCreation();
		await base.Create(campaign);
		return campaign;
	}

	public override async Task<Campaign> Update(Campaign campaign)
	{
		ValidateAllocationDestinations(campaign);
		var entity = await GetAndEnsureCreatedByCurrentUser(campaign.Id);
		entity.Update(campaign);
		return await base.Update(campaign);
	}

	private void ValidateAllocationDestinations(Campaign campaign)
		=> campaign.ActivatedPaymentMethods.ForEach(pm => _paymentMethodProvider.Get(pm.Identifier).ValidateAllocationDestination(pm.AllocationDestination));

	public async Task InitiateValidation(int campaignId)
	{
		var campaign = await GetAndEnsureCreatedByCurrentUser(campaignId);

		var balance = await GetTotalBalance(campaignId);
		campaign.TransitionToValidationPhase(balance);

		await _database.CommitChangesAsync();
	}

	public async Task AllocateAllValidated()
	{
		var campaigns = await _database.Campaigns
			.IncludeAll()
			.Where(campaign => campaign.AllocationId == null && campaign.ValidationId != null && campaign.Validation!.Expiration < DateTime.Now)
			.ToArrayAsync();

		foreach (var campaign in campaigns)
		{
			var votesStatus = await GetVotes(campaign.Id);

			var endorsedRatio = votesStatus.EndorsedBalance / votesStatus.Balance;

			if (double.IsNaN(endorsedRatio))
			{
				await Delete(campaign.Id);
				return;
			}

			await (endorsedRatio < votesStatus.ApprovalThreshold
				? campaign.Refund(_paymentMethodProvider)
				: campaign.Fund(_paymentMethodProvider));

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
		var balance = await GetTotalBalance(id);
		var endorsedDonations = 0d;
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
		campaign.EnsureNotInStatus(CampaignStatus.Allocation);
		await campaign.Refund(_paymentMethodProvider);
		return await base.Delete(id);
	}
}