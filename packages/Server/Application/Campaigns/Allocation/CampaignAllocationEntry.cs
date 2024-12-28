namespace Solidarity.Application.Campaigns.Allocation;

public enum CampaignAllocationEntryType { Fund, Refund }

public sealed class CampaignAllocationEntry : Entity
{
	public int CampaignAllocationId { get; set; }
	public CampaignAllocation CampaignAllocation { get; set; } = null!;
	public CampaignAllocationEntryType Type { get; set; }
	public string PaymentMethodIdentifier { get; set; } = null!;
	public string Data { get; set; } = null!;
	public decimal Amount { get; set; }
}