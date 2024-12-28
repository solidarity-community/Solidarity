namespace Solidarity.Application.Accounts;

public sealed class IsAccountAuthenticated(IAuthenticatedAccount authenticatedAccount, IDatabase database)
{
	public async Task<bool> Execute() => authenticatedAccount.Id is int id && await database.Accounts.AnyAsync(a => a.Id == id);
}