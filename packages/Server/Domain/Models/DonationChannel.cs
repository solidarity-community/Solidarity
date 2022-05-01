namespace Solidarity.Domain.Models;

public class DonationChannel : Model
{
	public string PaymentMethodIdentifier { get; set; } = null!;
	public int CampaignId { get; set; }
	public Campaign? Campaign { get; set; } = null!;
}