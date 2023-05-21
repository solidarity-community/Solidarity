using Throw;

namespace Solidarity.Application.Accounts;

public class IsAccountUsernameAvailable
{
	private readonly DatabaseContext _database;
	public IsAccountUsernameAvailable(DatabaseContext database) => _database = database;

	public async Task<bool> Execute(string username)
	{
		return string.IsNullOrWhiteSpace(username) is false
			&& await _database.Accounts.SingleOrDefaultAsync(a => a.Username == username.ToLower()) == null;
	}

	public async Task ExecuteAndThrow(string username)
	{
		var available = await Execute(username);
		available.Throw("Username is unavailable").IfFalse();
	}
}