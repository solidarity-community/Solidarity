namespace Solidarity.Application.Campaigns;

public sealed class CampaignExpenditure : Entity
{
	public int CampaignId { get; set; }
	public string Name { get; set; } = null!;
	public int Quantity { get; set; }
	public long UnitPrice { get; set; }
	public long TotalPrice => Quantity * UnitPrice;
}