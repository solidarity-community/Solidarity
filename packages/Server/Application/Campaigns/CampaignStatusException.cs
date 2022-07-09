namespace Solidarity.Application.Campaigns;

public class CampaignStatusException : Exception
{
	public CampaignStatusException(params CampaignStatus[] statuses)
		: base($"The campaign is not in the {string.Join(" or", statuses.Select(status => Enum.GetName(status)))} phase.") { }
}