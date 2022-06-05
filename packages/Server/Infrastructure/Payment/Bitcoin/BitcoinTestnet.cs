namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_TESTNET")]
public class BitcoinTestnet : Bitcoin
{
	public BitcoinTestnet(IDatabase database, ICurrentUserService currentUserService) : base(Network.TestNet, database, currentUserService) { }
}