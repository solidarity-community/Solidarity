namespace Solidarity.Application.Accounts;

public class GetAuthenticatedAccount
{
	private readonly ICurrentUserService _currentUserService;
	private readonly IDatabase _database;
	public GetAuthenticatedAccount(ICurrentUserService currentUserService, IDatabase database) => (_currentUserService, _database) = (currentUserService, database);

	public async Task<Account?> Execute()
	{
		return !_currentUserService.Id.HasValue || await _database.Accounts.GetOrDefault(_currentUserService.Id.Value) is not Account account
			? null
			: account.WithoutCredentials();
	}
}