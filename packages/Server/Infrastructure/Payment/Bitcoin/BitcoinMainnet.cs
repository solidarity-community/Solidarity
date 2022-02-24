namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_MAINNET")]
public class BitcoinMainnet : Bitcoin
{
	public BitcoinMainnet(IDatabase database) : base(Network.Main, database) { }
}