namespace Solidarity.Application.Campaigns;

public class CampaignAllocation : Model
{
	public Campaign Campaign { get; set; } = null!;
	public List<CampaignAllocationEntry> Entries { get; set; } = new();
}