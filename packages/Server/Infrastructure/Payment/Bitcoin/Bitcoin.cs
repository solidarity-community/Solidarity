namespace Solidarity.Infrastructure.Payment.Bitcoin;

public abstract class Bitcoin : PaymentMethod
{
	private readonly RPCClient _client;
	private readonly BitcoinExtKey _extendedPrivateKey;

	public Bitcoin(Network network, IDatabase database, ICurrentUserService currentUserService) : base(database, currentUserService)
	{
		if (network != Network.Main && network != Network.TestNet)
		{
			throw new NotImplementedException("Only Bitcoin Mainnet and Testnet networks are supported");
		}

		var server = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_SERVER");
		var username = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_USERNAME");
		var password = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_PASSWORD");
		_client = new(
			network: network,
			credentials: new() { Server = server, UserPassword = new(username, password) }
		);

		if (Key is null)
		{
			Key = new ExtKey().GetWif(_client.Network).ToWif();
		}

		_extendedPrivateKey = new BitcoinExtKey(Key, _client.Network);
	}

	public override async Task EnsureChannelCreated(int channelId)
	{
		var address = GetAddress(channelId);
		await _client.ImportAddressAsync(address);
	}

	public override Task<PaymentChannel> GetChannel(int channelId)
	{
		var key = GetKey(channelId);
		var channel = new BitcoinChannel(key, _client);
		return Task.FromResult(channel as PaymentChannel);
	}

	private Key GetKey(int channelId)
		=> _extendedPrivateKey.Derive(new KeyPath($"m/44'/{(_client.Network == Network.Main ? 0 : 1)}'/0'/{channelId}/{_currentUserService.Id ?? 0}")).PrivateKey;

	private BitcoinAddress GetAddress(int channelId)
		=> GetKey(channelId).PubKey.GetAddress(ScriptPubKeyType.Legacy, _client.Network);
}