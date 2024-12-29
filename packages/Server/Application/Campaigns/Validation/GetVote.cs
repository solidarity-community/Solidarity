namespace Solidarity.Application.Campaigns.Validation;

public sealed class GetVote(IDatabase database, IAuthenticatedAccount authenticatedAccount)
{
	public async Task<bool?> Execute(int campaignId)
	{
		var campaign = await database.Get<Campaign>(campaignId);
		return campaign.Validation?.Votes.Find(v => v.AccountId == authenticatedAccount.Id)?.Value;
	}
}