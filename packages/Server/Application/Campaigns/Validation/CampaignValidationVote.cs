namespace Solidarity.Application.Campaigns.Validation;

public class CampaignValidationVote : Entity
{
	public int ValidationId { get; set; }
	public CampaignValidation Validation { get; set; } = null!;

	public int AccountId { get; set; }
	public Account Account { get; set; } = null!;

	public bool Value { get; set; }
}