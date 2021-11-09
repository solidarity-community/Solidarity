namespace Solidarity.Domain.Exceptions;

public class AuthenticationFailedException : Exception
{
	public AuthenticationFailedException(string message = "Authentication Failed") : base(message) { }
}