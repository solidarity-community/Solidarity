namespace Solidarity.Infrastructure.Payment.Bitcoin;

public abstract class Bitcoin : PaymentMethod
{
	private readonly RPCClient _client;
	private readonly BitcoinExtKey _extendedPrivateKey;

	public Bitcoin(Network network, IDatabase database) : base(database)
	{
		if (network != Network.Main && network != Network.TestNet)
		{
			throw new NotImplementedException();
		}

		var server = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_SERVER");
		var username = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_USERNAME");
		var password = Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{Identifier}_PASSWORD");

		_client = new(
			new RPCCredentialString
			{
				Server = server,
				UserPassword = new NetworkCredential(username, password),
			},
			network
		);

		if (Key is null)
		{
			Key = new ExtKey().GetWif(_client.Network).ToWif();
		}

		_extendedPrivateKey = new BitcoinExtKey(Key, _client.Network);
	}

	public override PaymentChannel GetChannel(int channelId)
	{
		var bip44NetworkType = _client.Network == Network.Main ? "0" : "1";
		var key = _extendedPrivateKey.Derive(new KeyPath($"m/44'/{bip44NetworkType}'/0'/0/{channelId}")).PrivateKey;
		var address = key.PubKey.GetAddress(ScriptPubKeyType.Legacy, _client.Network);
		_client.ImportAddress(address);
		return new BitcoinChannel(key, _client);
	}
}