namespace Solidarity.Application.Accounts;

public sealed class GetAuthenticatedAccount(IAuthenticatedAccount authenticatedAccount, IDatabase database)
{
	public async Task<Account?> Execute()
	{
		return !authenticatedAccount.Id.HasValue || await database.GetOrDefault<Account>(authenticatedAccount.Id.Value) is not Account account
			? null
			: account.WithoutCredentials();
	}
}