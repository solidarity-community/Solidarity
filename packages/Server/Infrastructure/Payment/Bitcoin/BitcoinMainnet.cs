namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_MAINNET")]
public class BitcoinMainnet : Bitcoin
{
	public BitcoinMainnet(IDatabase database, ICurrentUserService currentUserService) : base(Network.Main, database, currentUserService) { }
}