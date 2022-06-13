namespace Solidarity.Infrastructure.Payment.Bitcoin;

public enum BitcoinTransactionSpeed { Fast = 1, Normal = 3, Economy = 6 }

public abstract class Bitcoin : PaymentMethod
{
	private const BitcoinTransactionSpeed Speed = BitcoinTransactionSpeed.Economy;

	private static readonly Dictionary<Network, int> CoinTypeByNetwork = new() {
		{ Network.Main, 0 },
		{ Network.TestNet, 1 },
	};

	public readonly RPCClient _client;
	public readonly BitcoinExtKey _extendedPrivateKey;

	public Bitcoin(Network network, IDatabase database) : base(database)
	{
		if (network != Network.Main && network != Network.TestNet)
		{
			throw new NotImplementedException("Only Bitcoin Mainnet and Testnet networks are supported");
		}

		var server = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_SERVER");
		var username = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_USERNAME");
		var password = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_PASSWORD");
		_client = new(
			network: network,
			credentials: new() { Server = server, UserPassword = new(username, password) }
		);

		if (Key is null)
		{
			Key = new ExtKey().GetWif(_client.Network).ToWif();
		}

		_extendedPrivateKey = new BitcoinExtKey(Key, _client.Network);

		_client.EnsureWalletCreated().GetAwaiter().GetResult();
	}

	public override async Task<string> GetDonationData(Campaign campaign, Account? account)
	{
		var address = await DeriveAddress(campaign, account);
		return address.ToString();
	}

	public override async Task<decimal> GetBalance(Campaign campaign, Account? account)
	{
		var utxos = await GetUTxOs(campaign, account);
		return utxos?.Select(utxo => utxo.Amount)
			.Aggregate(Money.Zero, (a, b) => a + b)
			.ToDecimal(MoneyUnit.Satoshi) ?? 0m;
	}

	public override async Task Refund(Campaign campaign, Account? account)
	{
		var utxos = await GetUTxOs(campaign, account);

		if (utxos.Any() == false)
		{
			return;
		}

		var feeRate = await GetFeeRate();
		await _client.Network.CreateTransactionBuilder()
			.AddKeys(_extendedPrivateKey)
			.RefundUTxOs(utxos)
			.SendEstimatedFeesSplit(feeRate)
			.BuildTransaction(sign: true)
			.SendAsync(_client);
	}

	public override async Task Allocate(Campaign campaign, string destination)
	{
		var destinationAddress = BitcoinAddress.Create(destination, _client.Network);
		var utxos = await GetUTxOs(campaign, null);
		var feeRate = await GetFeeRate();
		await _client.Network.CreateTransactionBuilder()
			.AddKeys(_extendedPrivateKey)
			.AddUTxOs(utxos)
			.SendEstimatedFeesSplit(feeRate)
			.SendAll(destinationAddress)
			.BuildTransaction(sign: true)
			.SendAsync(_client);
	}

	private async Task<FeeRate> GetFeeRate() => (await _client.TryEstimateSmartFeeAsync((int)Speed)).FeeRate;

	private Key DeriveKey(Campaign campaign, Account? account)
	{
		var keyPath = new KeyPath($"m/44'/{CoinTypeByNetwork.TryGet(_client.Network)}'/0'/{campaign.Id}/{account?.Id ?? 0}");
		return _extendedPrivateKey.Derive(keyPath).PrivateKey;
	}

	private async Task<BitcoinAddress> DeriveAddress(Campaign campaign, Account? account)
	{
		var key = DeriveKey(campaign, account);
		var address = key.PubKey.GetAddress(ScriptPubKeyType.Legacy, _client.Network);
		await _client.ImportAddressAsync(address, address.ToString(), false);
		return address;
	}

	private async Task<UnspentCoin[]> GetUTxOs(BitcoinAddress address)
	{
		return await _client.ListUnspentAsync(0, int.MaxValue, address);
	}

	private async Task<UnspentCoin[]> GetUTxOs(Campaign campaign, Account? account)
	{
		if (account is not null)
		{
			var address = await DeriveAddress(campaign, account);
			return await GetUTxOs(address);
		}
		else
		{
			var accounts = await _database.Accounts.ToListAsync();

			var publicUTxOsTask = GetUTxOs(await DeriveAddress(campaign, null));
			var accountsUTxOsTask = Task.WhenAll(accounts.Select(a => GetUTxOs(campaign, a)));

			await Task.WhenAll(publicUTxOsTask, accountsUTxOsTask);

			var publicUTxOs = publicUTxOsTask.Result;
			var accountsUTxOs = accountsUTxOsTask.Result.SelectMany(a => a).ToArray();

			return publicUTxOs.Concat(accountsUTxOs).ToArray();
		}
	}
}