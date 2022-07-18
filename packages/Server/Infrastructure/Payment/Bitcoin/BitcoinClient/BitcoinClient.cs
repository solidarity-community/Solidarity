namespace Solidarity.Infrastructure.Payment.Bitcoin;

public class BitcoinClient
{
	private const string WalletName = "wallet";
	private readonly RPCClient _client;
	public BitcoinClient(Bitcoin bitcoin)
	{
		var server = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{bitcoin.Identifier}_SERVER") ?? throw new InvalidOperationException($"PAYMENT_METHOD_{bitcoin.Identifier}_SERVER environment variable is not set");
		var username = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{bitcoin.Identifier}_USERNAME") ?? throw new InvalidOperationException($"PAYMENT_METHOD_{bitcoin.Identifier}_USERNAME environment variable is not set");
		var password = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{bitcoin.Identifier}_PASSWORD") ?? throw new InvalidOperationException($"PAYMENT_METHOD_{bitcoin.Identifier}_PASSWORD environment variable is not set");
		_client = new RPCClient(
			network: bitcoin.Network,
			credentials: new() { Server = server, UserPassword = new(username, password) }
		);
	}

	public Network Network => _client.Network;
	private TransactionBuilder CreateTransactionBuilder() => _client.Network.CreateTransactionBuilder();

	private bool isWalletLoaded = false;
	public async Task EnsureWalletCreated()
	{
		if (isWalletLoaded == true)
		{
			return;
		}

		try { await _client.CreateWalletAsync(WalletName); }
		catch (RPCException)
		{
			try { await _client.LoadWalletAsync(WalletName); }
			catch (RPCException) { }
		}
		finally { isWalletLoaded = true; }
	}

	public Task<BlockchainInfo> GetBlockchainInfoAsync(CancellationToken cancellationToken = default)
		=> _client.GetBlockchainInfoAsync(cancellationToken);

	public async Task<FeeRate> GetFeeRateAsync()
	{
		var smartEstimation = await _client.EstimateSmartFeeAsync(6, EstimateSmartFeeMode.Economical);
		return smartEstimation.FeeRate;
	}

	public RawTransaction FetchRawTransaction(string transactionId)
	{
		var transaction = _client.SendCommand("getrawtransaction", transactionId, true).Result.ToString();
		return JsonSerializer.Deserialize<RawTransaction>(transaction)!;
	}

	public async Task<UTxO[]> GetUTxOs(Key privateKey)
	{
		var address = await GetAndImportAddress(privateKey);
		var coins = await _client.ListUnspentAsync(0, int.MaxValue, address);
		return coins.Select(coin => new UTxO(privateKey, coin)).ToArray();
	}

	public BitcoinAddress GetAddress(Key privateKey)
	{
		return privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, _client.Network);
	}

	public async Task<BitcoinAddress> GetAndImportAddress(Key privateKey)
	{
		var address = GetAddress(privateKey);
		await _client.ImportAddressAsync(address, address.ToString(), false);
		return address;
	}

	public async Task<string> AllocateAndSend(Dictionary<UTxO, IEnumerable<BitcoinAllocation>> allocationsByUTxOs)
	{
		var totalAmount = allocationsByUTxOs.SelectMany(kvp => kvp.Value.Select(am => am.amount)).Sum();
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

		var maintainedFees = totalAmount - feeAdjustedAllocationsByUTxOs.SelectMany(kvp => kvp.Value.Select(refund => refund.amount)).Sum();

		var transaction = CreateTransactionBuilder()
			.Allocate(feeAdjustedAllocationsByUTxOs)
			.SendFeesSplit(maintainedFees)
			.BuildTransaction(sign: true);

		await _client.SendRawTransactionAsync(transaction);
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
		var totalAmount = allocationsByUTxOs.SelectMany(kvp => kvp.Value.Select(am => am.amount)).Sum();
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