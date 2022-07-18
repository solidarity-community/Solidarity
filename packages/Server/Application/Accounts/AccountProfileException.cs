namespace Solidarity.Application.Accounts;

public class AccountProfileException : Exception
{
	public AccountProfileException(string message = "This profile does not belong to this account") : base(message) { }
}