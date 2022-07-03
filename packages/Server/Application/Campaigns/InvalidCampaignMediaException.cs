namespace Solidarity.Application.Campaigns;

public class InvalidCampaignMediaException : Exception
{
	public InvalidCampaignMediaException(string message = "Invalid Campaign Media") : base(message) { }
}