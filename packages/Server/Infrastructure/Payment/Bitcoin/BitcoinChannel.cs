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

	public override async Task<decimal> GetBalance()
	{
		var utxos = await GetUTxOs(_address);
		return utxos
			.Select(utxo => utxo.Amount)
			.Aggregate(Money.Zero, (a, b) => a + b)
			.ToDecimal(MoneyUnit.Satoshi);
	}

	public override async Task Transfer(string destination, decimal amount)
	{
		var destinationAddress = BitcoinAddress.Create(destination, Network.Main);
		var builder = _client.Network.CreateTransactionBuilder();
		var utxos = await GetUTxOs(_address);
		utxos.ForEach(utxo => builder.AddCoins(utxo.AsCoin()));
		var feeRate = _client.TryEstimateSmartFee((int)Speed).FeeRate;
		builder.AddKeys(_privateKey).SendEstimatedFeesSplit(feeRate).SendAll(destinationAddress);
		var transaction = builder.BuildTransaction(true);
		_client.SendRawTransaction(transaction);
	}

	private async Task<List<UnspentCoin>> GetUTxOs(BitcoinAddress address)
	{
		var utxos = await _client.ListUnspentAsync(0, int.MaxValue, address);
		return utxos.ToList();
	}
}