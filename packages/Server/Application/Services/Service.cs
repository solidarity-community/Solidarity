namespace Solidarity.Application.Services;

public abstract class Service
{
	protected readonly IDatabase database;
	protected readonly ICryptoClientFactory cryptoClientFactory;
	protected readonly ICurrentUserService currentUserService;
	protected readonly ExtKey cryptoPrivateKey;

	public Service(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService)
	{
		this.database = database;
		this.cryptoClientFactory = cryptoClientFactory;
		this.currentUserService = currentUserService;
		cryptoPrivateKey = new Mnemonic(GetMnemonic().Mnemonic, Wordlist.English).DeriveExtKey();
	}

	private CryptoMnemonic GetMnemonic()
	{
		if (database.CryptoMnemonics.Any())
		{
			return database.CryptoMnemonics.First();
		}
		else
		{
			var mnemonics = new CryptoMnemonic(new Mnemonic(Wordlist.English, WordCount.TwentyFour).ToString());
			database.CryptoMnemonics.Add(mnemonics);
			database.CommitChanges();
			return mnemonics;
		}
	}
}