namespace Solidarity.Application.Authentication;

public class GetAuthenticationMethods
{
	private readonly IDatabase _database;
	private readonly CurrentUserService _currentUserService;
	public GetAuthenticationMethods(IDatabase database, CurrentUserService currentUserService) => (_database, _currentUserService) = (database, currentUserService);

	public async Task<Dictionary<AuthenticationMethodType, bool>> Execute()
	{
		var availableAuthenticationMethods = await _database.AuthenticationMethods
			.Where(am => am.AccountId == _currentUserService.Id)
			.Select(am => am.Type)
			.ToListAsync();

		return new Dictionary<AuthenticationMethodType, bool>
		{
			{ AuthenticationMethodType.Password, availableAuthenticationMethods.Contains(AuthenticationMethodType.Password) },
		};
	}
}