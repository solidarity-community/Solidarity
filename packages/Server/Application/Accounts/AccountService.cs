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
		return await _database.Accounts.SingleOrDefaultAsync(a => a.Username == username.ToLower()) == null;
	}

	public override async Task<Account> Create(Account account)
	{
		return string.IsNullOrWhiteSpace(account.Username) == false && await IsUsernameAvailable(account.Username) == false
			? throw new AccountTakenException("This username is not available")
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
		return await Reset(account.Id);
	}

	public async Task<string> Reset(int id)
	{
		var account = await Get(id);

		var phrase = RandomStringGenerator.Generate(64);

		_database.AccountRecoveryHandshakes.Add(new()
		{
			AccountId = account.Id,
			Phrase = phrase,
		});
		await _database.CommitChangesAsync();

		var publicRsaKey = RSA.Create();
		// TODO This fails
		publicRsaKey.ImportRSAPublicKey(Convert.FromBase64String(account.PublicKey), out _);
		// Encrypt Handshake with the public key of the SignDataToString(handshake.Phrase);
		var encryptedHandshake = publicRsaKey.SignDataToString(phrase) ?? throw new Exception("Cannot sign data");
		return encryptedHandshake;
	}

	public async Task<string> Recover(string phrase)
	{
		var handshake = await GetRecoveryHandshakeByPhrase(phrase);
		var token = handshake.Account.IssueToken(TimeSpan.FromDays(30));
		handshake.Account.AuthenticationMethods = new();
		_database.AuthenticationMethods.RemoveRange(_database.AuthenticationMethods.Where(e => e.AccountId == handshake.Account.Id));
		_database.AccountRecoveryHandshakes.Remove(handshake);
		await _database.CommitChangesAsync();
		return token;
	}

	public async Task<AccountRecoveryHandshake> GetRecoveryHandshakeByPhrase(string phrase)
	{
		_database.AccountRecoveryHandshakes
			.Where(e => e.Expiration <= DateTime.Now)
			.ToList()
			.ForEach(handshake => _database.AccountRecoveryHandshakes.Remove(handshake));
		await _database.CommitChangesAsync();
		return await _database.AccountRecoveryHandshakes.IncludeAll().FirstOrDefaultAsync(hs => hs.Phrase == phrase)
			?? throw new EntityNotFoundException<AccountRecoveryHandshake>();
	}
}