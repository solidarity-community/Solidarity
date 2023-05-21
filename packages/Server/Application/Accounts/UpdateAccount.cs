namespace Solidarity.Application.Accounts;

public class UpdateAccount
{
	private readonly IDatabase _database;
	private readonly ICurrentUserService _currentUserService;
	public UpdateAccount(IDatabase database, ICurrentUserService currentUserService) => (_database, _currentUserService) = (database, currentUserService);

	public async Task<Account> Execute(Account account)
	{
		_currentUserService.Id.ThrowIfNull("Not authenticated");
		account.Id = _currentUserService.Id.Value;
		await _database.Update(account);
		return account.WithoutCredentials();
	}
}