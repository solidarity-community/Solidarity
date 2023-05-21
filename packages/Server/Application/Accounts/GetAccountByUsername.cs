namespace Solidarity.Application.Accounts;

public class GetAccountByUsername
{
	private readonly IDatabase _database;
	public GetAccountByUsername(IDatabase database) => _database = database;
	public Task<Account> Execute(string username) => _database.Accounts.Get(account => account.Username == username);
}