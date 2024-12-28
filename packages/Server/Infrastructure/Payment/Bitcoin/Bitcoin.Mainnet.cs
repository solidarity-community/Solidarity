namespace Solidarity.Infrastructure.Payment.Bitcoin;

[PaymentMethod("BTC_MAINNET")]
public sealed class BitcoinMainnet(IDatabase database, ILogger<BitcoinMainnet> logger) : Bitcoin(Network.Main, database, logger)
{
}