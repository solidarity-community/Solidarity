namespace Solidarity.Application.Campaigns.Allocation;

public class CampaignAllocation : Entity
{
	public Campaign Campaign { get; set; } = null!;
	public List<CampaignAllocationEntry> Entries { get; set; } = new();
}