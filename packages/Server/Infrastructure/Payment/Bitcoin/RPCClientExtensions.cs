namespace Solidarity.Infrastructure.Payment.Bitcoin;

public static class RPCClientExtensions
{
	private const string WalletName = "wallet";
	private static bool IsWalletLoaded = false;

	public static async Task EnsureWalletCreated(this RPCClient client)
	{
		if (IsWalletLoaded == true)
		{
			return;
		}

		try { await client.CreateWalletAsync(WalletName); }
		catch (RPCException)
		{
			try { await client.LoadWalletAsync(WalletName); }
			catch (RPCException) { }
		}
		finally { IsWalletLoaded = true; }
	}

	public static TransactionBuilder AddUTxOs(this TransactionBuilder txBuilder, UTxO[] utxos)
	{
		foreach (var utxo in utxos)
		{
			txBuilder
				.AddKeys(utxo.PrivateKey)
				.AddCoin(utxo.Coin.AsCoin());
		}
		return txBuilder;
	}

	public static TransactionBuilder RefundUTxOs(this TransactionBuilder txBuilder, RPCClient client, UTxO[] utxos)
	{
		var refundsByUTxOs = utxos.Select(utxo =>
		{
			var utxoTransactionId = utxo.Coin.OutPoint.Hash.ToString();
			var utxoTransactionSum = utxo.Coin.Amount.Satoshi;
			var utxoTransaction = client.FetchRawTransaction(utxoTransactionId);

			var spentCoins = utxoTransaction.vin.Select(input =>
			{
				var originatedTransaction = client.FetchRawTransaction(input.txid);
				var spentCoinData = originatedTransaction.vout.Find(output => output.n == input.vout);
				return new
				{
					Address = spentCoinData!.scriptPubKey.address,
					Amount = new Money(spentCoinData.value, MoneyUnit.BTC).Satoshi,
				};
			}).ToList();

			decimal originatedTransactionsSum = spentCoins.Sum(coin => coin.Amount);

			return (utxo, spentCoins.Select(spentCoin =>
			{
				var distribution = spentCoin.Amount / originatedTransactionsSum;
				var bitcoinAddress = BitcoinAddress.Create(spentCoin.Address, client.Network);
				var amount = Math.Floor(utxoTransactionSum * distribution);
				return (bitcoinAddress, amount);
			}));
		}).ToDictionary(x => x.utxo, x => x.Item2);

		var estimatedTransactionSize = txBuilder.Network.CreateTransactionBuilder()
			.AddRefunds(refundsByUTxOs)
			.BuildTransaction(sign: true)
			.GetVirtualSize();
		var feeRate = client.GetFeeRate();
		var estimatedTransactionFee = feeRate.GetFee(estimatedTransactionSize);

		var totalInputAmount = refundsByUTxOs.Select(kvp => kvp.Key.Coin.Amount.Satoshi).Sum();
		var totalAmount = refundsByUTxOs.SelectMany(kvp => kvp.Value.Select(am => am.amount)).Sum();
		var remainingAmountDueToRounding = new Money((long)(totalInputAmount - totalAmount));

		if (estimatedTransactionFee.ToDecimal(MoneyUnit.Satoshi) >= totalAmount)
		{
			throw new Exception("Estimated transaction fee is greater than total amount");
		}

		var feeAmountToSubtract = remainingAmountDueToRounding >= estimatedTransactionFee
			? Money.Zero
			: estimatedTransactionFee - remainingAmountDueToRounding;

		var feeAdjustedRefundsByUTxOs = refundsByUTxOs.Select(kvp =>
		{
			var (utxo, refunds) = kvp;
			var adjustedRefunds = refunds.Select(refund =>
			{
				var (bitcoinAddress, amount) = refund;
				var weightedFee = Math.Ceiling(feeAmountToSubtract.ToDecimal(MoneyUnit.Satoshi) * amount / totalAmount);
				var adjustedAmount = amount - weightedFee;
				return (bitcoinAddress, adjustedAmount);
			});
			return (utxo, adjustedRefunds);
		}).ToDictionary(x => x.utxo, x => x.adjustedRefunds);

		var maintainedFees = totalAmount - feeAdjustedRefundsByUTxOs.SelectMany(kvp => kvp.Value.Select(refund => refund.adjustedAmount)).Sum();
		return txBuilder.AddRefunds(feeAdjustedRefundsByUTxOs).SendFeesSplit(Money.FromUnit(maintainedFees, MoneyUnit.Satoshi));
	}

	private static TransactionBuilder AddRefunds(this TransactionBuilder txBuilder, Dictionary<UTxO, IEnumerable<(BitcoinAddress bitcoinAddress, decimal amount)>> refundsByUTxOs)
	{
		foreach (var (utxo, refunds) in refundsByUTxOs)
		{
			foreach (var (bitcoinAddress, amount) in refunds)
			{
				txBuilder
					.AddKeys(utxo.PrivateKey)
					.AddCoin(utxo.Coin.AsCoin())
					.Send(bitcoinAddress, new Money(amount, MoneyUnit.Satoshi));
			}
		}
		return txBuilder;
	}

	public static async Task<string> SignAndSendAsync(this TransactionBuilder txBuilder, RPCClient client)
	{
		var transaction = txBuilder.BuildTransaction(sign: true);
		await transaction.SendAsync(client);
		return transaction.GetHash().ToString();
	}

	public static async Task<FeeRate> GetFeeRateAsync(this RPCClient client)
	{
		var smartEstimation = await client.EstimateSmartFeeAsync(6, EstimateSmartFeeMode.Economical);
		return smartEstimation.FeeRate;
	}

	public static FeeRate GetFeeRate(this RPCClient client)
	{
		return client.GetFeeRateAsync().GetAwaiter().GetResult();
	}

	public static RawTransaction FetchRawTransaction(this RPCClient client, string transactionId)
	{
		var transaction = client.SendCommand("getrawtransaction", transactionId, true).Result.ToString();
		return JsonSerializer.Deserialize<RawTransaction>(transaction)!;
	}

	public static Task<uint256> SendAsync(this Transaction transaction, RPCClient client)
	{
		return client.SendRawTransactionAsync(transaction);
	}

	public static async Task<UTxO[]> GetUTxOs(this RPCClient client, Key privateKey)
	{
		var address = await client.GetAndImportAddress(privateKey);
		var coins = await client.ListUnspentAsync(0, int.MaxValue, address);
		return coins.Select(coin => new UTxO(privateKey, coin)).ToArray();
	}

	public static async Task<BitcoinAddress> GetAndImportAddress(this RPCClient client, Key privateKey)
	{
		var address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, client.Network);
		await client.ImportAddressAsync(address, address.ToString(), false);
		return address;
	}
}
