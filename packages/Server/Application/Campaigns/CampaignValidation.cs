namespace Solidarity.Application.Campaigns;

public class CampaignValidation : Model
{
	public const double ApprovalThresholdPercentage = 0.75f;
	private const int ValidationDays = 7;

	public Campaign Campaign { get; set; } = null!;

	public List<CampaignValidationVote> Votes { get; set; } = new();

	public DateTime Expiration { get; set; } = DateTime.Now.AddDays(ValidationDays);

	public double ApprovalThreshold => ApprovalThresholdPercentage;
}