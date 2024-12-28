namespace Solidarity.Application.Campaigns;

public sealed class CampaignPaymentMethod : Entity
{
	public string Identifier { get; set; } = null!;
	public int CampaignId { get; set; }
	public Campaign? Campaign { get; set; }
	public string AllocationDestination { get; set; } = null!;
}