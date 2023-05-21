namespace Solidarity.Application.Campaigns.Validation;

public class GetVote
{
	private readonly IDatabase _database;
	private readonly ICurrentUserService _currentUserService;
	public GetVote(IDatabase database, ICurrentUserService currentUserService) => (_database, _currentUserService) = (database, currentUserService);

	public async Task<bool?> Execute(int campaignId)
	{
		var campaign = await _database.Campaigns.Get(campaignId);
		return campaign.Validation?.Votes.Find(v => v.AccountId == _currentUserService.Id)?.Value;
	}
}
