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
}
