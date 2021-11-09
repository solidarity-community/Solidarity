namespace Solidarity.Infrastructure.Crypto;

public class CryptoClient
{
	public Network Network { get; set; }
	private Uri Server { get; set; }
	private NetworkCredential RPCCredentials { get; set; }
	private int BIP44CoinType { get; set; }

	private RPCCredentialString Credentials => new() { UserPassword = RPCCredentials };
	private RPCClient RPC => new(Credentials, Server, Network);

	public CryptoClient(Uri server, NetworkCredential credentials, Network network, int bip44CoinType = 1)
	{
		Server = server;
		RPCCredentials = credentials;
		Network = network;
		BIP44CoinType = bip44CoinType;
	}

	public IDestination GetAddress(Key privateKey)
	{
		var address = privateKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network);
		TrackAddress(address);
		return address;
	}

	public Key DeriveKey(ExtKey privateKey, int addressIndex)
	{
		return privateKey.Derive(new KeyPath($"m/44'/{BIP44CoinType}'/0'/0/{addressIndex}")).PrivateKey;
	}

	public void TrackAddress(IDestination address, string? label = null)
	{
		RPC.ImportAddress(address, label, false);
	}

	public FeeRate EstimateFee(TransactionSpeed transactionSpeed)
	{
		return RPC.TryEstimateSmartFee((int)transactionSpeed).FeeRate;
	}

	public List<UnspentCoin> GetUTxOs(BitcoinAddress address)
	{
		return RPC.ListUnspent(0, int.MaxValue, address).ToList();
	}

	public Money GetBalance(string addressStr)
	{
		var address = BitcoinAddress.Create(addressStr, Network);
		var utxos = GetUTxOs(address);
		var balance = utxos.Select(utxo => utxo.Amount).Aggregate(seed: Money.Zero, func: (a, b) => a + b);
		return balance;
	}

	public uint256 ReleaseFunds(Key origin, IDestination destination, TransactionSpeed transactionSpeed = TransactionSpeed.Economy)
	{
		var builder = Network.CreateTransactionBuilder();
		var utxos = GetUTxOs(origin.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network));
		utxos.ForEach(utxo => builder.AddCoins(utxo.AsCoin()));
		builder.AddKeys(origin).SendEstimatedFeesSplit(EstimateFee(transactionSpeed)).SendAll(destination);
		var transaction = builder.BuildTransaction(true);
		return RPC.SendRawTransaction(transaction);
	}
}