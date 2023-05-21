namespace Solidarity.Application.Accounts.Profiles;

public class CreateOrUpdateAccountProfile
{
	private readonly IDatabase _database;
	private readonly ICurrentUserService _currentUserService;
	public CreateOrUpdateAccountProfile(IDatabase database, ICurrentUserService currentUserService) => (_database, _currentUserService) = (database, currentUserService);

	public async Task<AccountProfile> Execute(AccountProfile profile)
	{
		return await _database.AccountProfiles.AnyAsync(i => i.AccountId == profile.AccountId)
			? await Update(profile)
			: await Create(profile);
	}

	private Task<AccountProfile> Create(AccountProfile profile) => _database.Create(profile);

	private async Task<AccountProfile> Update(AccountProfile profile)
	{
		(profile.AccountId != _currentUserService.Id).Throw().IfTrue();
		return await _database.Update(profile);
	}
}