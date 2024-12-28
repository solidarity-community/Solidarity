namespace Solidarity.Application.Campaigns;

public sealed class CampaignStatusException(params CampaignStatus[] statuses)
	: Exception($"The campaign is in the {string.Join(" or ", statuses.Select(status => Enum.GetName(status)))} phase.")
{
}