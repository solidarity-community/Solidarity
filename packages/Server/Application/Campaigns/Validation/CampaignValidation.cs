
namespace Solidarity.Application.Campaigns.Validation;

public static class VotesExtensions
{
	public static bool? GetByAccountId(this List<CampaignValidationVote> votes, int? accountId)
		=> votes.Find(v => v.AccountId == accountId)?.Value;

	public static void Cast(this List<CampaignValidationVote> votes, int accountId, bool value)
	{
		var vote = votes.Find(v => v.AccountId == accountId);
		if (vote is null)
		{
			vote = new() { AccountId = accountId };
			votes.Add(vote);
		}
		vote.Value = value;
	}
}

public sealed class CampaignValidation : Entity
{
	public const double ApprovalThresholdPercentage = 0.75f;
	private const int ValidationDays = 7;

	internal static void ConfigureDatabase(ModelBuilder b)
	{
		b.Entity<CampaignValidation>().HasMany(v => v.Votes).WithOne(v => v.Validation);
		b.Entity<CampaignValidationVote>().HasOne(v => v.Account).WithMany().HasForeignKey(v => v.AccountId);
	}

	public Campaign Campaign { get; set; } = null!;
	[AutoInclude] public List<CampaignValidationVote> Votes { get; set; } = [];

	public DateTime Expiration { get; set; } = DateTime.Now.AddDays(ValidationDays);
	public double ApprovalThreshold => ApprovalThresholdPercentage;
}