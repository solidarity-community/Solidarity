namespace Solidarity.Application.Accounts;

public sealed class UpdateAccount(IDatabase database, IAuthenticatedAccount authenticatedAccount)
{
	public async Task<Account> Execute(Account account)
	{
		authenticatedAccount.Id.ThrowIfNull("Not authenticated");
		account.Id = authenticatedAccount.Id.Value;
		await database.Update(account);
		return account.WithoutCredentials();
	}
}