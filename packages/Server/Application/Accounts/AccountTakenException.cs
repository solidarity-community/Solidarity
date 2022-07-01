namespace Solidarity.Application.Accounts;

[MapToHttpStatusCode(HttpStatusCode.Conflict)]
public class AccountTakenException : Exception
{
	public AccountTakenException(string message = "This account has already been taken") : base(message) { }
}