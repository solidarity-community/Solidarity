namespace Solidarity.Domain.Models;

public enum CampaignMediaType { File, YouTube, Twitch }

public class CampaignMedia : Model
{
	private static readonly Regex YouTubeVideoIdRegex = new(@"^[a-zA-Z0-9_-]{11}$");
	private static readonly Regex TwitchVideoIdRegex = new(@"^[0-9]{10}$");

	public int CampaignId { get; set; }
	public CampaignMediaType Type { get; set; }
	public string Uri { get; set; } = null!;

	public bool IsValid()
	{
		return Type switch
		{
			CampaignMediaType.File => Uri.StartsWith("/"),
			CampaignMediaType.YouTube => YouTubeVideoIdRegex.IsMatch(Uri),
			CampaignMediaType.Twitch => TwitchVideoIdRegex.IsMatch(Uri),
			_ => throw new InvalidOperationException("Invalid media type"),
		};
	}
}