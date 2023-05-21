namespace Solidarity.Application.Accounts;

public class GetAccount
{
	private readonly IDatabase _database;
	public GetAccount(IDatabase database) => _database = database;

	public async Task<Account> Execute(int id)
	{
		var account = await _database.Accounts.Get(id);
		return account.WithoutCredentials();
	}
}