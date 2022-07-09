namespace Solidarity.Application.Campaigns;

public enum CampaignAllocationEntryType { Fund, Refund }

public class CampaignAllocationEntry : Model
{
	public int CampaignAllocationId { get; set; }
	public CampaignAllocation CampaignAllocation { get; set; } = null!;
	// public CampaignAllocationEntryType Type { get; set; }
	public string PaymentMethodIdentifier { get; set; } = null!;
	public string Data { get; set; } = null!;
	// public decimal Amount { get; set; }
}