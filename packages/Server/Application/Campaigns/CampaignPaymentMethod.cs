namespace Solidarity.Application.Campaigns;

public class CampaignPaymentMethod : Model
{
	public string Identifier { get; set; } = null!;
	public int CampaignId { get; set; }
	public Campaign? Campaign { get; set; } = null!;
}