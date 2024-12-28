namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_TESTNET")]
public sealed class BitcoinTestnet(IDatabase database, ILogger<BitcoinTestnet> logger) : Bitcoin(Network.TestNet, database, logger)
{
}