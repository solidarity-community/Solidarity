namespace Solidarity.Application.Campaigns.Media;

public enum CampaignMediaType { File, YouTube, Twitch }

public partial class CampaignMedia : Entity
{
	[GeneratedRegex("^[a-zA-Z0-9_-]{11}$")] private static partial Regex YouTubeVideoIdRegex();
	[GeneratedRegex("^[0-9]{10}$")] private static partial Regex TwitchVideoIdRegex();

	public int CampaignId { get; set; }
	public CampaignMediaType Type { get; set; }
	public string Uri { get; set; } = null!;

	public bool IsValid()
	{
		return Type switch
		{
			CampaignMediaType.File => Uri.StartsWith("/"),
			CampaignMediaType.YouTube => YouTubeVideoIdRegex().IsMatch(Uri),
			CampaignMediaType.Twitch => TwitchVideoIdRegex().IsMatch(Uri),
			_ => false
		};
	}

	public void Validate() => IsValid().Throw("Invalid media").IfFalse();
}