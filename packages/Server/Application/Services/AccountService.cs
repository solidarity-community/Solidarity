namespace Solidarity.Application.Services;

public class AccountService : CrudService<Account>
{
	private readonly HandshakeService handshakeService;

	public AccountService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService, HandshakeService handshakeService) : base(database, cryptoClientFactory, currentUserService)
	{
		this.handshakeService = handshakeService;
	}

	public Account Get()
	{
		return base.Get(currentUserService.Id ?? throw new NotAuthenticatedException());
	}

	public Account GetWithoutAuthentication(int? id)
	{
		var account = id.HasValue
			? Get(id.Value)
			: Get();
		return account.WithoutAuthenticationData();
	}

	public Account GetByUsername(string username)
	{
		return database.Accounts.SingleOrDefault(account => account.Username == username)
			?? throw new EntityNotFoundException<Account>();
	}

	public bool IsUsernameAvailable(string username)
	{
		return database.Accounts.SingleOrDefault(a => a.Username == username.ToLower()) == null;
	}

	public override Account Create(Account account)
	{
		if (string.IsNullOrWhiteSpace(account.Username) == false && IsUsernameAvailable(account.Username) == false)
		{
			throw new AccountTakenException("This username is not available");
		}

		return base.Create(account);
	}

	public string CreateAndIssueToken(Account account)
	{
		Create(account);
		return account.IssueToken(TimeSpan.FromDays(30));
	}

	public override Account Update(Account account)
	{
		account.Id = currentUserService.Id ?? throw new NotAuthenticatedException();
		base.Update(account);
		return account.WithoutAuthenticationData();
	}

	public string ResetByUsername(string username)
	{
		var account = GetByUsername(username);
		return Reset(account.Id);
	}

	public string Reset(int id)
	{
		var account = Get(id);

		var handshake = new Handshake
		{
			AccountId = account.Id,
			Phrase = RandomStringGenerator.Generate(64),
			Expiration = DateTime.Now.AddMinutes(10)
		};
		database.Handshakes.Add(handshake);
		database.CommitChanges();

		var publicRsaKey = RSA.Create();
		// TODO This fails
		publicRsaKey.ImportRSAPublicKey(Convert.FromBase64String(account.PublicKey), out _);
		// Encrypt Handshake with the public key of the SignDataToString(handshake.Phrase);
		var encryptedHandshake = publicRsaKey.SignDataToString(handshake.Phrase) ?? throw new Exception("Cannot sign data");
		return encryptedHandshake;
	}

	public string Recover(string phrase)
	{
		var handshake = handshakeService.GetByPhrase(phrase);
		var token = handshake.Account.IssueToken(TimeSpan.FromDays(30));
		handshake.Account.AuthenticationMethods = new();
		database.AuthenticationMethods.RemoveRange(database.AuthenticationMethods.Where(e => e.AccountId == handshake.Account.Id));
		database.Handshakes.Remove(handshake);
		database.CommitChanges();
		return token;
	}
}