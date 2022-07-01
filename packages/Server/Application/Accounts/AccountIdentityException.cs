namespace Solidarity.Application.Accounts;

public class AccountIdentityException : Exception
{
	public AccountIdentityException(string message) : base(message) { }
}