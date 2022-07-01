namespace Solidarity.Application.Campaigns;

public class InvalidMediaException : Exception
{
	public InvalidMediaException(string message = "Invalid Media") : base(message) { }
}