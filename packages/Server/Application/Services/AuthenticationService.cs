namespace Solidarity.Application.Services;

public class AuthenticationService : Service
{
	private readonly AccountService _accountService;

	public AuthenticationService(AccountService accountService, IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService)
		=> _accountService = accountService;

	public bool IsAuthenticated()
	{
		return _currentUserService.Id is int id
			&& _accountService.Exists(id);
	}

	public Dictionary<AuthenticationMethodType, bool> GetAll()
	{
		var availableAuthenticationMethods = _database.AuthenticationMethods
			.Where(am => am.AccountId == _currentUserService.Id)
			.Select(am => am.Type);

		return new Dictionary<AuthenticationMethodType, bool>
		{
			{ AuthenticationMethodType.Password, availableAuthenticationMethods.Contains(AuthenticationMethodType.Password) },
		};
	}

	public T AddOrUpdate<T>(T authentication) where T : AuthenticationMethod
	{
		var account = _accountService.Get(authentication.AccountId);
		authentication.Encrypt();
		if (account.AuthenticationMethods.SingleOrDefault(auth => auth is T) is not T existingAuthentication)
		{
			_database.AuthenticationMethods.Add(authentication);
			_database.CommitChanges();
			return authentication.WithoutData();
		}
		else
		{
			existingAuthentication.Data = authentication.Data;
			_database.CommitChanges();
			return existingAuthentication.WithoutData();
		}
	}

	public string Login<T>(int accountId, string data) where T : AuthenticationMethod
	{
		return _database.AuthenticationMethods.SingleOrDefault(am => am is T && am.AccountId == accountId) is not T authMethod || !authMethod.Authenticate(data)
			? throw new IncorrectCredentialsException()
			: _accountService.Get(accountId).IssueToken(TimeSpan.FromDays(30));
	}

	public void UpdatePassword(string newPassword, string? oldPassword = null)
	{
		var accountId = _currentUserService.Id ?? throw new NotAuthenticatedException();
		var existingAuthentication = _database.AuthenticationMethods.FirstOrDefault(am => am is PasswordAuthentication && am.AccountId == accountId);

		if (string.IsNullOrEmpty(oldPassword) == false && existingAuthentication?.Authenticate(oldPassword) == false)
		{
			throw new IncorrectCredentialsException();
		}

		AddOrUpdate(new PasswordAuthentication
		{
			AccountId = accountId,
			Data = newPassword,
		});
	}

	public string PasswordLogin(string username, string password)
	{
		var account = _accountService.GetByUsername(username);
		var token = Login<PasswordAuthentication>(account.Id, password);
		return token;
	}
}