namespace Solidarity.Application.Accounts;

public class CreateAccount
{
	private readonly IDatabase _database;
	private readonly IsAccountUsernameAvailable _isAccountUsernameAvailable;
	public CreateAccount(IDatabase database, IsAccountUsernameAvailable isAccountUsernameAvailable) => (_database, _isAccountUsernameAvailable) = (database, isAccountUsernameAvailable);

	public async Task<string> Execute(Account account)
	{
		await _isAccountUsernameAvailable.ExecuteAndThrow(account.Username);
		await _database.Create(account);
		return account.IssueToken(TimeSpan.FromDays(30));
	}
}