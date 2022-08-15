namespace Solidarity.Application.Accounts;

[MapToHttpStatusCode(HttpStatusCode.Conflict)]
public class UsernameUnavailableException : InvalidOperationException
{
	public UsernameUnavailableException(string message = "This username is unavailable") : base(message) { }
}