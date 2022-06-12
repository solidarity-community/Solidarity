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

	public Key DeriveKey(Campaign campaign, Account? account)
	{
		var keyPath = GetKeyPathFor(campaign, account);
		return _extendedPrivateKey.Derive(keyPath).PrivateKey;
	}

	public async Task<BitcoinAddress> DeriveAddress(Campaign campaign, Account? account)
	{
		var key = DeriveKey(campaign, account);
		var address = key.PubKey.GetAddress(ScriptPubKeyType.Legacy, _client.Network);
		await _client.ImportAddressAsync(address, address.ToString(), false);
		return address;
	}

	public override async Task<decimal> GetBalance(Campaign campaign, Account? account)
	{
		if (account is not null)
		{
			var address = await DeriveAddress(campaign, account);
			var utxos = await GetUTxOs(address);
			return AggregateUTxOs(utxos);
		}
		else
		{
			var publicBalanceTask = GetUTxOs(await DeriveAddress(campaign, null));
			var accountsBalanceTask = Task.WhenAll(_database.Accounts.ToList().Select(a => GetBalance(campaign, a)));
			await Task.WhenAll(publicBalanceTask, accountsBalanceTask);

			var publicBalance = AggregateUTxOs(publicBalanceTask.Result);
			var accountsBalance = accountsBalanceTask.Result.Sum();

			return publicBalance + accountsBalance;
		}
	}

	public async Task<UnspentCoin[]> GetUTxOs(BitcoinAddress address)
	{
		return await _client.ListUnspentAsync(0, int.MaxValue, address);
	}

	public override async Task Withdraw(Campaign campaign, string destination, decimal amount)
	{
		var sourceAddress = await DeriveAddress(campaign, null); // false address
		var destinationAddress = _extendedPrivateKey.GetPublicKey().GetAddress(ScriptPubKeyType.Legacy, _client.Network);
		var builder = _client.Network.CreateTransactionBuilder();
		var utxos = await GetUTxOs(sourceAddress);
		utxos.ToList().ForEach(utxo => builder.AddCoins(utxo.AsCoin()));
		var feeRate = _client.TryEstimateSmartFee((int)Speed).FeeRate;
		builder.AddKeys(_extendedPrivateKey).SendEstimatedFeesSplit(feeRate).SendAll(destinationAddress);
		var transaction = builder.BuildTransaction(true);
		_client.SendRawTransaction(transaction);
	}

	public override async Task<string> GetDonationData(Campaign campaign, Account? account)
	{
		var address = await DeriveAddress(campaign, account);
		return address.ToString();
	}

	public KeyPath GetKeyPathFor(Campaign campaign, Account? account)
		=> new($"m/44'/{CoinTypeByNetwork.TryGet(_client.Network)}'/0'/{campaign.Id}/{account?.Id ?? 0}");

	private decimal AggregateUTxOs(IEnumerable<UnspentCoin> utxos)
		=> utxos?.Select(utxo => utxo.Amount)
			.Aggregate(Money.Zero, (a, b) => a + b)
			.ToDecimal(MoneyUnit.Satoshi) ?? 0;
}