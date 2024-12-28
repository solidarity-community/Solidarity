namespace Solidarity.Application.Campaigns.Validation;

public sealed class GetVotes(IDatabase database, IPaymentMethodProvider paymentMethodProvider, GetCampaignBalanceShare getBalanceShare)
{
	public async Task<CampaignVotes> Execute(int campaignId)
	{
		var campaign = await database.Get<Campaign>(campaignId);
		campaign.AssertNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);
		var balance = await campaign.GetTotalBalance(paymentMethodProvider);
		var endorsedDonations = 0d;
		foreach (var vote in campaign.Validation!.Votes.Where(vote => vote.Value == true))
		{
			endorsedDonations += await getBalanceShare.Execute(campaignId, vote.AccountId);
		}
		return new(endorsedDonations, balance, campaign.Validation!.ApprovalThreshold);
	}
}
