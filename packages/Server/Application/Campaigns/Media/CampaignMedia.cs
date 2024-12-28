
namespace Solidarity.Application.Campaigns.Media;

public enum CampaignMediaType { File, YouTube, Twitch }

public partial class CampaignMedia : Entity, IValidatableObject
{
	[GeneratedRegex("^[a-zA-Z0-9_-]{11}$")] private static partial Regex YouTubeVideoIdRegex { get; }
	[GeneratedRegex("^[0-9]{10}$")] private static partial Regex TwitchVideoIdRegex { get; }

	public int CampaignId { get; set; }
	public CampaignMediaType Type { get; set; }
	public string Uri { get; set; } = null!;

	private bool IsValid()
	{
		return Type switch
		{
			CampaignMediaType.File => Uri.StartsWith('/'),
			CampaignMediaType.YouTube => YouTubeVideoIdRegex.IsMatch(Uri),
			CampaignMediaType.Twitch => TwitchVideoIdRegex.IsMatch(Uri),
			_ => false
		};
	}

	IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
	{
		if (!IsValid())
		{
			yield return new ValidationResult("Invalid media", [nameof(Uri)]);
		}
	}
}