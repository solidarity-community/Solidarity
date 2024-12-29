namespace Solidarity.Application.Accounts;

public sealed class IsAccountUsernameAvailable(DatabaseContext database)
{
	public async Task<bool> Execute(string username)
	{
		return string.IsNullOrWhiteSpace(username) is false
			&& await database.Accounts.SingleOrDefaultAsync(a => a.Username == username) is null;
	}

	public async Task ExecuteAndThrow(string username)
	{
		var available = await Execute(username);
		available.Throw("Username is unavailable").IfFalse();
	}
}