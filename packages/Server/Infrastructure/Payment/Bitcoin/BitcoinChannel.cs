namespace Solidarity.Infrastructure.Payment.Bitcoin;

public enum BitcoinTransactionSpeed { Fast = 1, Normal = 3, Economy = 6 }

public class BitcoinChannel : PaymentChannel
{
	private const BitcoinTransactionSpeed Speed = BitcoinTransactionSpeed.Economy;

	private readonly RPCClient _client;
	private readonly Key _privateKey;
	private readonly BitcoinAddress _address;

	public BitcoinChannel(Key privateKey, RPCClient client)
	{
		_privateKey = privateKey;
		_address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
		_client = client;
	}

	public override Task<decimal> GetBalance()
	{
		var utxos = _client.ListUnspent(0, int.MaxValue, _address).ToList();
		return Task.FromResult(GetUTxOs(_address)
			.Select(utxo => utxo.Amount)
			.Aggregate(
				seed: Money.Zero,
				func: (a, b) => a + b
			).ToDecimal(MoneyUnit.Satoshi)
		);
	}

	public override Task Transfer(string destination, decimal amount)
	{
		var destinationAddress = BitcoinAddress.Create(destination, Network.Main);
		var builder = _client.Network.CreateTransactionBuilder();
		var utxos = GetUTxOs(_address);
		utxos.ForEach(utxo => builder.AddCoins(utxo.AsCoin()));
		var feeRate = _client.TryEstimateSmartFee((int)Speed).FeeRate;
		builder.AddKeys(_privateKey).SendEstimatedFeesSplit(feeRate).SendAll(destinationAddress);
		var transaction = builder.BuildTransaction(true);
		_client.SendRawTransaction(transaction);
		return Task.CompletedTask;
	}

	private List<UnspentCoin> GetUTxOs(BitcoinAddress address)
	{
		return _client.ListUnspent(0, int.MaxValue, address).ToList();
	}
}