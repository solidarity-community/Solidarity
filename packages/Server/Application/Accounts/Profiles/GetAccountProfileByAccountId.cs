namespace Solidarity.Application.Accounts.Profiles;

public class GetAccountProfile
{
	private readonly IDatabase _database;
	private readonly CurrentUserService _currentUserService;
	public GetAccountProfile(IDatabase database, CurrentUserService currentUserService) => (_database, _currentUserService) = (database, currentUserService);

	public async Task<AccountProfile?> ExecuteAuthenticated() => _currentUserService.Id is null ? null : await ExecuteByAccountId(_currentUserService.Id.Value);

	public Task<AccountProfile> ExecuteByAccountId(int accountId) => _database.AccountProfiles.Get(ap => ap.AccountId == accountId);

	public Task<AccountProfile> Execute(int id) => _database.AccountProfiles.Get(id);
}