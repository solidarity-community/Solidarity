namespace Solidarity.Infrastructure.Payment.Bitcoin;

public abstract class Bitcoin : PaymentMethod
{
	public static readonly Dictionary<Network, int> CoinTypeByNetwork = new() {
		{ Network.Main, 0 },
		{ Network.TestNet, 1 },
	};

	public override string Name => $"Bitcoin {(Client.Network == Network.Main ? "" : "Testnet")}";

	public BitcoinClient Client { get; }
	public BitcoinExtKey ExtendedPrivateKey { get; }
	public Network Network { get; }

	public Bitcoin(Network network, IDatabase database) : base(database)
	{
		if (network != Network.Main && network != Network.TestNet)
		{
			throw new InvalidOperationException("Only Bitcoin Mainnet and Testnet networks are supported");
		}

		Network = network;
		Client = new(this);

		if (Key is null)
		{
			Key = new ExtKey().GetWif(Client.Network).ToWif();
		}

		ExtendedPrivateKey = new BitcoinExtKey(Key, network);

		try { Client.EnsureWalletCreated().GetAwaiter().GetResult(); }
		catch { }
	}

	public override async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var blockchainInfo = await Client.GetBlockchainInfoAsync(cancellationToken);
			return blockchainInfo.InitialBlockDownload
				? HealthCheckResult.Degraded($"{Name} is currently downloading the blockchain at {blockchainInfo.VerificationProgress * 100}%")
				: HealthCheckResult.Healthy();
		}
		catch
		{
			return HealthCheckResult.Unhealthy($"No connection to the {Name} RPC");
		}
	}

	public override PaymentChannel GetChannel(Campaign campaign) => new BitcoinChannel(this, campaign);
}