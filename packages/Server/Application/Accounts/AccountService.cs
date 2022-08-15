namespace Solidarity.Application.Accounts;

[TransientService]
public class AccountService : CrudService<Account>
{
	public AccountService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public async Task<Account?> Get()
	{
		return !_currentUserService.Id.HasValue ? null : await base.Get(_currentUserService.Id.Value);
	}

	public async Task<Account?> GetWithoutAuthentication()
	{
		var account = await Get();
		return account?.WithoutAuthenticationData();
	}

	public async Task<Account> GetByUsername(string username)
	{
		return await _database.Accounts.SingleOrDefaultAsync(account => account.Username == username)
			?? throw new EntityNotFoundException<Account>();
	}

	public async Task<bool> IsUsernameAvailable(string username)
	{
		return string.IsNullOrWhiteSpace(username) is false
			&& await _database.Accounts.SingleOrDefaultAsync(a => a.Username == username.ToLower()) == null;
	}

	public override async Task<Account> Create(Account account)
	{
		return await IsUsernameAvailable(account.Username) == false
			? throw new UsernameUnavailableException()
			: await base.Create(account);
	}

	public async Task<string> CreateAndIssueToken(Account account)
	{
		await Create(account);
		return account.IssueToken(TimeSpan.FromDays(30));
	}

	public override async Task<Account> Update(Account account)
	{
		account.Id = _currentUserService.Id ?? throw new NotAuthenticatedException();
		await base.Update(account);
		return account.WithoutAuthenticationData();
	}

	public async Task<string> ResetByUsername(string username)
	{
		var account = await GetByUsername(username);

		var phrase = RandomStringGenerator.Generate(64);

		var publicRsaKey = RSA.Create();
		publicRsaKey.ImportFromPem(account.PublicKey);
		var encryptedPhrase = Convert.ToBase64String(publicRsaKey.Encrypt(Encoding.ASCII.GetBytes(phrase), RSAEncryptionPadding.Pkcs1));

		_database.AccountRecoveryHandshakes.Add(new()
		{
			AccountId = account.Id,
			Phrase = phrase,
		});
		await _database.CommitChangesAsync();

		return encryptedPhrase;
	}

	public async Task<string> Recover(string phrase)
	{
		foreach (var expiredHandshake in _database.AccountRecoveryHandshakes.Where(e => e.Expiration <= DateTime.Now))
		{
			_database.AccountRecoveryHandshakes.Remove(expiredHandshake);
		}

		await _database.CommitChangesAsync();

		var handshake = await _database.AccountRecoveryHandshakes.IncludeAll().FirstOrDefaultAsync(hs => hs.Phrase == phrase)
			?? throw new EntityNotFoundException<AccountRecoveryHandshake>();

		var token = handshake.Account.IssueToken(TimeSpan.FromDays(30));

		handshake.Account.AuthenticationMethods = new();
		_database.AuthenticationMethods.RemoveRange(_database.AuthenticationMethods.Where(e => e.AccountId == handshake.AccountId));
		_database.AccountRecoveryHandshakes.Remove(handshake);

		await _database.CommitChangesAsync();
		return token;
	}
}