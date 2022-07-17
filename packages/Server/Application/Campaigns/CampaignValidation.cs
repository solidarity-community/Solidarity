namespace Solidarity.Application.Campaigns;

public class CampaignValidation : Model
{
	private const int ValidationDays = 7;
	private const double ApprovalThresholdPercentage = 0.75f;

	public Campaign Campaign { get; set; } = null!;

	public List<CampaignValidationVote> Votes { get; set; } = new();

	public DateTime Expiration { get; set; } = DateTime.Now.AddDays(ValidationDays);

	public double ApprovalThreshold => ApprovalThresholdPercentage;
}