namespace Solidarity.Domain.Models;

public class CampaignExpenditure : Model
{
	public int CampaignId { get; set; }
	public string Name { get; set; } = null!;
	public int Quantity { get; set; }
	public long UnitPrice { get; set; }
	public long TotalPrice => Quantity * UnitPrice;
}