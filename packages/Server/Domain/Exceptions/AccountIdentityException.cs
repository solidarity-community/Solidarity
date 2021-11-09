namespace Solidarity.Domain.Exceptions;

public class AccountIdentityException : Exception
{
	public AccountIdentityException(string message) : base(message) { }
}