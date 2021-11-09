
namespace Solidarity.Application.Common;

public interface ICryptoClientFactory
{
	CryptoClient GetClient(CoinType coinType, NetworkType networkType);
}