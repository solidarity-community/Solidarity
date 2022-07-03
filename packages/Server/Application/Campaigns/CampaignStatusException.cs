namespace Solidarity.Application.Campaigns;

public class CampaignStatusException : Exception
{
	public CampaignStatusException(CampaignStatus status) : base($"The campaign is not in the {Enum.GetName(status)} phase.") { }
}