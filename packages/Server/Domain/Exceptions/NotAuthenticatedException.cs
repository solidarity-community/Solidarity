namespace Solidarity.Domain.Exceptions;

public class NotAuthenticatedException : Exception
{
	public NotAuthenticatedException(string message = "Not authenticated") : base(message) { }
}