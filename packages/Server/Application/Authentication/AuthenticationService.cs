namespace Solidarity.Application.Authentication;

[TransientService]
public class AuthenticationService : Service
{
	private readonly AccountService _accountService;

	public AuthenticationService(AccountService accountService, IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService)
		=> _accountService = accountService;

	public async Task<bool> IsAuthenticated()
	{
		return _currentUserService.Id is int id
			&& await _accountService.Exists(id);
	}

	public async Task<Dictionary<AuthenticationMethodType, bool>> GetAll()
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

	public async Task<T> AddOrUpdate<T>(T authentication) where T : AuthenticationMethod
	{
		var account = await _accountService.Get(authentication.AccountId);
		authentication.Encrypt();
		if (account.AuthenticationMethods.SingleOrDefault(auth => auth is T) is not T existingAuthentication)
		{
			_database.AuthenticationMethods.Add(authentication);
			await _database.CommitChangesAsync();
			return authentication.WithoutData();
		}
		else
		{
			existingAuthentication.Data = authentication.Data;
			await _database.CommitChangesAsync();
			return existingAuthentication.WithoutData();
		}
	}

	public async Task<string> Login<T>(int accountId, string data) where T : AuthenticationMethod
	{
		return _database.AuthenticationMethods.SingleOrDefault(am => am is T && am.AccountId == accountId) is not T authMethod || !authMethod.Authenticate(data)
			? throw new IncorrectCredentialsException()
			: (await _accountService.Get(accountId)).IssueToken(TimeSpan.FromDays(30));
	}

	public async Task UpdatePassword(string newPassword, string? oldPassword = null)
	{
		var accountId = _currentUserService.Id ?? throw new NotAuthenticatedException();
		var existingAuthentication = _database.AuthenticationMethods.FirstOrDefault(am => am is PasswordAuthenticationMethod && am.AccountId == accountId);

		if (string.IsNullOrEmpty(oldPassword) == false && existingAuthentication?.Authenticate(oldPassword) == false)
		{
			throw new IncorrectCredentialsException();
		}

		await AddOrUpdate(new PasswordAuthenticationMethod
		{
			AccountId = accountId,
			Data = newPassword,
		});
	}

	public async Task<string> PasswordLogin(string username, string password)
	{
		var account = await _accountService.GetByUsername(username);
		var token = await Login<PasswordAuthenticationMethod>(account.Id, password);
		return token;
	}
}