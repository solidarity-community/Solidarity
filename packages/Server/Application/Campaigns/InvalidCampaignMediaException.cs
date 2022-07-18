namespace Solidarity.Application.Campaigns;

public class InvalidCampaignMediaException : InvalidOperationException
{
	public InvalidCampaignMediaException(string message = "Invalid Campaign Media") : base(message) { }
}