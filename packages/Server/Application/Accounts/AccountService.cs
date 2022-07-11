namespace Solidarity.Application.Accounts;

[TransientService]
public class AccountService : CrudService<Account>
{
	private readonly HandshakeService _handshakeService;

	public AccountService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, HandshakeService handshakeService) : base(database, paymentMethodProvider, currentUserService)
		=> _handshakeService = handshakeService;

	public async Task<Account?> Get()
	{
		return !_currentUserService.Id.HasValue ? null : await base.Get(_currentUserService.Id.Value);
	}

	public async Task<Account?> GetWithoutAuthentication(int? id)
	{
		var account = id.HasValue ? await Get(id.Value) : await Get();
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

		var handshake = new Handshake
		{
			AccountId = account.Id,
			Phrase = RandomStringGenerator.Generate(64),
			Expiration = DateTime.Now.AddMinutes(10)
		};
		_database.Handshakes.Add(handshake);
		await _database.CommitChangesAsync();

		var publicRsaKey = RSA.Create();
		// TODO This fails
		publicRsaKey.ImportRSAPublicKey(Convert.FromBase64String(account.PublicKey), out _);
		// Encrypt Handshake with the public key of the SignDataToString(handshake.Phrase);
		var encryptedHandshake = publicRsaKey.SignDataToString(handshake.Phrase) ?? throw new Exception("Cannot sign data");
		return encryptedHandshake;
	}

	public async Task<string> Recover(string phrase)
	{
		var handshake = await _handshakeService.GetByPhrase(phrase);
		var token = handshake.Account.IssueToken(TimeSpan.FromDays(30));
		handshake.Account.AuthenticationMethods = new();
		_database.AuthenticationMethods.RemoveRange(_database.AuthenticationMethods.Where(e => e.AccountId == handshake.Account.Id));
		_database.Handshakes.Remove(handshake);
		await _database.CommitChangesAsync();
		return token;
	}
}