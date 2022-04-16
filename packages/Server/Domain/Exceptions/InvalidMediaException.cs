namespace Solidarity.Domain.Exceptions;

public class InvalidMediaException : Exception
{
	public InvalidMediaException(string message = "Invalid Media") : base(message) { }
}