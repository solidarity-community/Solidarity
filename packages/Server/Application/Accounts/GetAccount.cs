namespace Solidarity.Application.Accounts;

public sealed class GetAccount(IDatabase database)
{
	public async Task<Account> Execute(int id)
	{
		var account = await database.Get<Account>(id);
		return account.WithoutCredentials();
	}
}