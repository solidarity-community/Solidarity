namespace Solidarity.Infrastructure.Payment.Bitcoin;

public class BitcoinChannel : PaymentChannel
{
	private readonly BitcoinExtKey _extendedPrivateKey;
	private readonly BitcoinClient _client;

	public BitcoinChannel(Bitcoin paymentMethod, Campaign campaign) : base(paymentMethod, campaign)
	{
		_extendedPrivateKey = paymentMethod.ExtendedPrivateKey;
		_client = paymentMethod.Client;
	}

	public override async Task<string> GetDonationData(Account? account)
	{
		var address = await DeriveAndImportAddress(account);
		return address.ToString();
	}

	public override async Task<decimal> GetBalance(Account? account)
	{
		var utxos = await GetUTxOs(account);
		return utxos?.Select(utxo => utxo.Coin.Amount)
			.Aggregate(Money.Zero, (a, b) => a + b)
			.ToDecimal(MoneyUnit.Satoshi) ?? 0m;
	}

	public override async Task<IEnumerable<CampaignAllocationEntry>> Allocate()
	{
		var healthCheckResult = await _paymentMethod.CheckHealthAsync();
		if (healthCheckResult.Status == HealthStatus.Degraded)
		{
			throw new InvalidOperationException($"{_paymentMethod.Name} is degraded and cannot perform any non-read-only operations as this would cause a fund loss. {healthCheckResult.Description}");
		}

		if (!_allocationList.RefundRemaining && !_allocationList.FundRemaining)
		{
			throw new NotImplementedException("Allocation without specifying remaining action not implemented yet");
		}

		var utxos = await GetUTxOs();

		bool ShallFund(UTxO utxo)
		{
			var shallFund = _allocationList.FundRemaining || _allocationList.Funds.Any(a => DeriveAddress(a) == utxo.Coin.Address);
			var shallRefund = _allocationList.RefundRemaining || _allocationList.Refunds.Any(a => DeriveAddress(a) == utxo.Coin.Address);
			return (shallFund, shallRefund) switch
			{
				(_, true) => false,
				(true, _) => true,
				_ => throw new InvalidOperationException("ShallFund and ShallRefund are both false")
			};
		}

		var allocationsByUTxOs = utxos
			.Where(utxo => _allocationList.RefundRemaining || _allocationList.FundRemaining || _allocationList.All.Select(a => DeriveAddress(a)).Contains(utxo.Coin.Address))
			.Select(utxo => (utxo, ShallFund(utxo) ? AllocateFund(utxo) : AllocateRefund(utxo)))
			.ToDictionary(x => x.utxo, x => x.Item2);

		var transactionHash = await _client.AllocateAndSend(allocationsByUTxOs);

		return allocationsByUTxOs.Select(x => new CampaignAllocationEntry
		{
			PaymentMethodIdentifier = _paymentMethod.Identifier,
			Type = ShallFund(x.Key) ? CampaignAllocationEntryType.Fund : CampaignAllocationEntryType.Refund,
			Amount = x.Value.Sum(y => y.amount),
			Data = transactionHash,
		});
	}

	private IEnumerable<BitcoinAllocation> AllocateRefund(UTxO utxo)
	{
		var utxoTransaction = _client.FetchRawTransaction(utxo.Coin.OutPoint.Hash.ToString());
		var utxoTransactionSum = utxo.Coin.Amount.Satoshi;

		var donations = utxoTransaction.vin.Select(input =>
		{
			var originatedTransaction = _client.FetchRawTransaction(input.txid);
			var originatedTransactionOutput = originatedTransaction.vout.Find(output => output.n == input.vout);
			return new
			{
				Address = BitcoinAddress.Create(originatedTransactionOutput!.scriptPubKey.address, _client.Network),
				Amount = new Money(originatedTransactionOutput.value, MoneyUnit.BTC).Satoshi,
			};
		}).ToList();

		decimal donationsSum = donations.Sum(coin => coin.Amount);

		// Bitcoin transactions also contain a fee, which are not deducted from the donation inputs.
		// Therefore the total amount of "donations" is almost always greater than the actual received amount.
		// So we can calculate the real amount via "utxoTransactionSum" and every donation's weight.
		return donations.Select(donation =>
		{
			var weight = donation.Amount / donationsSum;
			var amount = Money.FromUnit(Math.Floor(utxoTransactionSum * weight), MoneyUnit.Satoshi);
			return new BitcoinAllocation(donation.Address, amount);
		});
	}

	private IEnumerable<BitcoinAllocation> AllocateFund(UTxO utxo)
	{
		var fundAllocationAddress = BitcoinAddress.Create(AllocationDestination, _client.Network);
		return new List<BitcoinAllocation> { (fundAllocationAddress, utxo.Coin.Amount) };
	}

	private BitcoinAddress DeriveAddress(Account? account)
	{
		var key = DeriveKey(account);
		return _client.GetAddress(key);
	}

	private async Task<BitcoinAddress> DeriveAndImportAddress(Account? account)
	{
		var key = DeriveKey(account);
		return await _client.GetAndImportAddress(key);
	}

	private Key DeriveKey(Account? account)
	{
		var keyPath = new KeyPath($"m/44'/{Bitcoin.CoinTypeByNetwork.TryGet(_client.Network)}'/0'/{_campaign.Id}/{account?.Id ?? 0}");
		return _extendedPrivateKey.Derive(keyPath).PrivateKey;
	}

	private async Task<UTxO[]> GetUTxOs(Account? account = null)
	{
		if (account is not null)
		{
			var key = DeriveKey(account);
			return await _client.GetUTxOs(key);
		}
		else
		{
			var accounts = await _database.Accounts.ToListAsync();

			var key = DeriveKey(null);
			var publicUTxOsTask = _client.GetUTxOs(key);
			var accountsUTxOsTask = Task.WhenAll(accounts.Select(a => GetUTxOs(a)));

			await Task.WhenAll(publicUTxOsTask, accountsUTxOsTask);

			var publicUTxOs = publicUTxOsTask.Result;
			var accountsUTxOs = accountsUTxOsTask.Result.SelectMany(a => a).ToArray();

			return publicUTxOs.Concat(accountsUTxOs).ToArray();
		}
	}
}