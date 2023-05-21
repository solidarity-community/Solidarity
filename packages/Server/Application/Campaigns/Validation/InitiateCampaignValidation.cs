namespace Solidarity.Application.Campaigns.Validation;

public class InitiateCampaignValidation
{
	private readonly IDatabase _database;
	private readonly ICurrentUserService _currentUserService;
	private readonly GetCampaignBalance _getBalance;
	public InitiateCampaignValidation(IDatabase database, ICurrentUserService currentUserService, GetCampaignBalance getBalance)
		=> (_database, _currentUserService, _getBalance) = (database, currentUserService, getBalance);

	public async Task Execute(int campaignId)
	{
		var campaign = await _database.Campaigns.Get(campaignId);
		campaign.EnsureCreatedByCurrentUser(_currentUserService);

		var balance = await _getBalance.Execute(campaignId);
		campaign.TransitionToValidationPhase(balance);

		await _database.CommitChangesAsync();
	}
}
