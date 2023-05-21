namespace Solidarity.Application.Campaigns.Validation;

public class GetVotes
{
	private readonly IDatabase _database;
	private readonly GetCampaignBalance _getBalance;
	private readonly GetCampaignBalanceShare _getBalanceShare;
	public GetVotes(IDatabase database, GetCampaignBalance getBalance, GetCampaignBalanceShare getBalanceShare)
		=> (_database, _getBalance, _getBalanceShare) = (database, getBalance, getBalanceShare);

	public async Task<CampaignVotes> Execute(int campaignId)
	{
		var campaign = await _database.Campaigns.Get(campaignId);
		campaign.EnsureNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);
		var balance = await _getBalance.Execute(campaignId);
		var endorsedDonations = 0d;
		foreach (var vote in campaign.Validation!.Votes.Where(vote => vote.Value == true))
		{
			endorsedDonations += await _getBalanceShare.Execute(campaignId, vote.AccountId);
		}
		return new(endorsedDonations, balance, campaign.Validation!.ApprovalThreshold);
	}
}
