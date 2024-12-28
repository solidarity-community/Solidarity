namespace Solidarity.Application.Accounts;

public sealed class GetAccountByUsername(IDatabase database)
{
	public Task<Account> Execute(string username) => database.Get<Account>(account => account.Username == username);
}