namespace Solidarity.Application.Accounts;

public sealed class CreateAccount(IDatabase database, IsAccountUsernameAvailable isAccountUsernameAvailable)
{
	public async Task<string> Execute(Account account)
	{
		await isAccountUsernameAvailable.ExecuteAndThrow(account.Username);
		await database.Create(account);
		return account.IssueToken(TimeSpan.FromDays(30));
	}
}