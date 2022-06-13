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

	public static TransactionBuilder AddUTxOs(this TransactionBuilder txBuilder, UnspentCoin[] utxos)
	{
		foreach (var utxo in utxos)
		{
			txBuilder.AddCoin(utxo.AsCoin());
		}
		return txBuilder;
	}

	public static TransactionBuilder RefundUTxOs(this TransactionBuilder txBuilder, UnspentCoin[] utxos)
	{
		foreach (var utxo in utxos)
		{
			txBuilder.AddCoin(utxo.AsCoin()).Send(utxo.Address, utxo.Amount);
		}
		return txBuilder;
	}

	public static Task<uint256> SendAsync(this Transaction transaction, RPCClient client)
	{
		return client.SendRawTransactionAsync(transaction);
	}
}
