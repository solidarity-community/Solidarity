namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_TESTNET")]
public class BitcoinTestnet : Bitcoin
{
	public BitcoinTestnet(IDatabase database) : base(Network.TestNet, database) { }
}