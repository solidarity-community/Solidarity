namespace Solidarity.Application.Campaigns;

public class CampaignValidation : Model
{
	public Campaign Campaign { get; set; } = null!;

	public List<CampaignValidationVote> Votes { get; set; } = new();

	public DateTime Expiration { get; set; }
}