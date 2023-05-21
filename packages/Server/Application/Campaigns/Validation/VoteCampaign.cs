namespace Solidarity.Application.Campaigns.Validation;

public class VoteCampaign
{
	private readonly IDatabase _database;
	private readonly ICurrentUserService _currentUserService;
	public VoteCampaign(IDatabase database, ICurrentUserService currentUserService) => (_database, _currentUserService) = (database, currentUserService);

	public async Task Execute(int campaignId, bool vote)
	{
		var campaign = await _database.Campaigns.Get(campaignId);
		campaign.Vote(_currentUserService.Id!.Value, vote);
		await _database.CommitChangesAsync();
	}
}
