
using Solidarity.Infrastructure.Crypto;
using Solidarity.Domain.Enums;

namespace Solidarity.Application.Common
{
	public interface ICryptoClientFactory
	{
		CryptoClient GetClient(CoinType coinType, NetworkType networkType);
	}
}