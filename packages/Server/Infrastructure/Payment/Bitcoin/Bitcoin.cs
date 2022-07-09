namespace Solidarity.Infrastructure.Payment.Bitcoin;

public abstract class Bitcoin : PaymentMethod
{
	private static readonly Dictionary<Network, int> CoinTypeByNetwork = new() {
		{ Network.Main, 0 },
		{ Network.TestNet, 1 },
	};

	public override string Name => $"Bitcoin {(_client.Network == Network.Main ? "" : "Testnet")}";

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

		try { _client.EnsureWalletCreated().GetAwaiter().GetResult(); }
		catch { }
	}

	public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) => CheckHealthAsync(cancellationToken);

	private async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var blockchainInfo = await _client.GetBlockchainInfoAsync(cancellationToken);
			return blockchainInfo.InitialBlockDownload
				? HealthCheckResult.Degraded($"{Name} is currently downloading the blockchain at {blockchainInfo.VerificationProgress * 100}%")
				: HealthCheckResult.Healthy();
		}
		catch
		{
			return HealthCheckResult.Unhealthy($"No connection to the {Name} RPC");
		}
	}

	public override async Task<string> GetDonationData(Campaign campaign, Account? account)
	{
		var address = await DeriveAddress(campaign, account);
		return address.ToString();
	}

	public override async Task<decimal> GetBalance(Campaign campaign, Account? account)
	{
		var utxos = await GetUTxOs(campaign, account);
		return utxos?.Select(utxo => utxo.Coin.Amount)
			.Aggregate(Money.Zero, (a, b) => a + b)
			.ToDecimal(MoneyUnit.Satoshi) ?? 0m;
	}

	public override async Task Refund(Campaign campaign, Account? account)
	{
		await EnsureHealthForNonReadOnlyOperation();
		var utxos = await GetUTxOs(campaign, account);

		if (utxos.Any() == false)
		{
			return;
		}

		await _client.Network.CreateTransactionBuilder()
			.RefundUTxOs(_client, utxos)
			.SignAndSendAsync(_client);
	}

	public override async Task<string> Fund(Campaign campaign, string destination)
	{
		await EnsureHealthForNonReadOnlyOperation();
		var destinationAddress = BitcoinAddress.Create(destination, _client.Network);
		var utxos = await GetUTxOs(campaign, null);
		return await _client.Network.CreateTransactionBuilder()
			.AddUTxOs(utxos)
			.SendAll(destinationAddress)
			.SignAndSendAsync(_client);
	}

	private async Task EnsureHealthForNonReadOnlyOperation()
	{
		var healthCheckResult = await CheckHealthAsync();
		if (healthCheckResult.Status == HealthStatus.Degraded)
		{
			throw new Exception($"{Name} is degraded and cannot perform any non-read-only operations as this would cause a fund loss. {healthCheckResult.Description}");
		}
	}

	private Key DeriveKey(Campaign campaign, Account? account)
	{
		var keyPath = new KeyPath($"m/44'/{CoinTypeByNetwork.TryGet(_client.Network)}'/0'/{campaign.Id}/{account?.Id ?? 0}");
		return _extendedPrivateKey.Derive(keyPath).PrivateKey;
	}

	private async Task<BitcoinAddress> DeriveAddress(Campaign campaign, Account? account)
	{
		var key = DeriveKey(campaign, account);
		return await _client.GetAndImportAddress(key);
	}

	private async Task<UTxO[]> GetUTxOs(Campaign campaign, Account? account)
	{
		if (account is not null)
		{
			var key = DeriveKey(campaign, account);
			return await _client.GetUTxOs(key);
		}
		else
		{
			var accounts = await _database.Accounts.ToListAsync();

			var key = DeriveKey(campaign, null);
			var publicUTxOsTask = _client.GetUTxOs(key);
			var accountsUTxOsTask = Task.WhenAll(accounts.Select(a => GetUTxOs(campaign, a)));

			await Task.WhenAll(publicUTxOsTask, accountsUTxOsTask);

			var publicUTxOs = publicUTxOsTask.Result;
			var accountsUTxOs = accountsUTxOsTask.Result.SelectMany(a => a).ToArray();

			return publicUTxOs.Concat(accountsUTxOs).ToArray();
		}
	}
}