namespace Solidarity.Infrastructure.Payment.Bitcoin;

public sealed class BitcoinClient
{
	private const string WalletName = "wallet";

	private readonly RPCClient client;

	public string Name => $"Bitcoin {(client.Network == Network.Main ? "" : "Testnet")}";

	public BitcoinClient(Bitcoin bitcoin, ILogger logger)
	{
		var (server, username, password) = EnvironmentVariables.GetPaymentMethod(bitcoin.Identifier).Deconstruct();
		client = new RPCClient(
			network: bitcoin.Network,
			credentials: new() { Server = server, UserPassword = new(username, password) }
		);
		logger.LogInformation("Initialized connection to {Name} with username {username} and password {password}", Name, username, $"{password[..2]}**{password[^2..]}");
	}

	public Network Network => client.Network;
	private TransactionBuilder CreateTransactionBuilder() => client.Network.CreateTransactionBuilder();

	private bool isWalletLoaded;
	public async Task EnsureWalletCreated()
	{
		if (isWalletLoaded == true)
		{
			return;
		}

		try { await client.CreateWalletAsync(WalletName, new() { Descriptors = false }); }
		catch (RPCException)
		{
			try { await client.LoadWalletAsync(WalletName); }
			catch (RPCException) { }
		}
		finally { isWalletLoaded = true; }
	}

	public Task<BlockchainInfo> GetBlockchainInfoAsync(CancellationToken cancellationToken = default)
		=> client.GetBlockchainInfoAsync(cancellationToken);

	public async Task<FeeRate> GetFeeRateAsync()
	{
		var smartEstimation = await client.EstimateSmartFeeAsync(6, EstimateSmartFeeMode.Economical);
		return smartEstimation.FeeRate;
	}

	public RawTransaction FetchRawTransaction(string transactionId)
	{
		var transaction = client.SendCommand("getrawtransaction", transactionId, true).Result.ToString();
		return JsonSerializer.Deserialize<RawTransaction>(transaction)!;
	}

	public async Task<UTxO[]> GetUTxOs(Key privateKey)
	{
		var address = await GetAndImportAddress(privateKey);
		var coins = await client.ListUnspentAsync(0, int.MaxValue, address);
		return coins.Select(coin => new UTxO(privateKey, coin)).ToArray();
	}

	public BitcoinAddress GetAddress(Key privateKey)
	{
		return privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, client.Network);
	}

	public async Task<BitcoinAddress> GetAndImportAddress(Key privateKey)
	{
		var address = GetAddress(privateKey);
		await client.ImportAddressAsync(address, address.ToString(), false);
		return address;
	}

	public async Task<string> AllocateAndSend(Dictionary<UTxO, IEnumerable<BitcoinAllocation>> allocationsByUTxOs)
	{
		var totalAmount = allocationsByUTxOs.SelectMany(kvp => kvp.Value.Select(am => am.Amount)).Sum();
		var totalFees = CalculateAndSendFeesSplit(allocationsByUTxOs);

		var feeAdjustedAllocationsByUTxOs = allocationsByUTxOs.Select(kvp =>
		{
			var (utxo, allocations) = kvp;
			var adjustedAllocations = allocations.Select(allocation =>
			{
				var (bitcoinAddress, amount) = allocation;
				var weightedFee = Math.Ceiling(totalFees.ToDecimal(MoneyUnit.Satoshi) * amount.ToDecimal(MoneyUnit.Satoshi) / totalAmount.ToDecimal(MoneyUnit.Satoshi));
				var adjustedAmount = amount.ToDecimal(MoneyUnit.Satoshi) - weightedFee;
				return new BitcoinAllocation(bitcoinAddress, new Money(adjustedAmount, MoneyUnit.Satoshi));
			});
			return (utxo, adjustedAllocations);
		}).ToDictionary(x => x.utxo, x => x.adjustedAllocations);

		var maintainedFees = totalAmount - feeAdjustedAllocationsByUTxOs.SelectMany(kvp => kvp.Value.Select(refund => refund.Amount)).Sum();

		var transaction = CreateTransactionBuilder()
			.Allocate(feeAdjustedAllocationsByUTxOs)
			.SendFeesSplit(maintainedFees)
			.BuildTransaction(sign: true);

		await client.SendRawTransactionAsync(transaction);
		return transaction.GetHash().ToString();
	}

	private Money CalculateAndSendFeesSplit(Dictionary<UTxO, IEnumerable<BitcoinAllocation>> allocationsByUTxOs)
	{
		var txBuilder = CreateTransactionBuilder();
		var estimatedTransactionSize = txBuilder
			.Allocate(allocationsByUTxOs)
			.BuildTransaction(sign: true)
			.GetVirtualSize();
		var feeRate = GetFeeRateAsync().GetAwaiter().GetResult();
		var estimatedTransactionFee = feeRate.GetFee(estimatedTransactionSize);
		var totalInputAmount = allocationsByUTxOs.Select(kvp => kvp.Key.Coin.Amount.Satoshi).Sum();
		var totalAmount = allocationsByUTxOs.SelectMany(kvp => kvp.Value.Select(am => am.Amount)).Sum();
		var remainingAmountDueToRounding = new Money((long)(totalInputAmount - totalAmount));
		return estimatedTransactionFee >= totalAmount
			? throw new InvalidOperationException("Estimated transaction fee is greater than total amount")
			: remainingAmountDueToRounding >= estimatedTransactionFee
				? Money.Zero
				: estimatedTransactionFee - remainingAmountDueToRounding;
	}
}


public static class RPCClientExtensions
{
	public static TransactionBuilder Allocate(this TransactionBuilder txBuilder, Dictionary<UTxO, IEnumerable<BitcoinAllocation>> allocationsByUTxOs)
	{
		foreach (var (utxo, allocation) in allocationsByUTxOs)
		{
			foreach (var (bitcoinAddress, amount) in allocation)
			{
				txBuilder
					.AddKeys(utxo.PrivateKey)
					.AddCoin(utxo.Coin.AsCoin())
					.Send(bitcoinAddress, amount);
			}
		}
		return txBuilder;
	}
}