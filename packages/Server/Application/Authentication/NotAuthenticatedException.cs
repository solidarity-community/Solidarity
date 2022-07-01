namespace Solidarity.Application.Authentication;

[MapToHttpStatusCode(HttpStatusCode.Unauthorized)]
public class NotAuthenticatedException : Exception
{
	public NotAuthenticatedException(string message = "Not authenticated") : base(message) { }
}