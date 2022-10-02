namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_MAINNET")]
public class BitcoinMainnet : Bitcoin
{
	public BitcoinMainnet(IDatabase database, ILogger<BitcoinMainnet> logger) : base(Network.Main, database, logger) { }
}