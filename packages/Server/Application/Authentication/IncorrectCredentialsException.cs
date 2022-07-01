namespace Solidarity.Application.Authentication;

[MapToHttpStatusCode(HttpStatusCode.Unauthorized)]
public class IncorrectCredentialsException : Exception
{
	public IncorrectCredentialsException(string message = "Incorrect Credentials") : base(message) { }
}