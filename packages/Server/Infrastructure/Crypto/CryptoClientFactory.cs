using Solidarity.Application.Common;
using Solidarity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net;

namespace Solidarity.Infrastructure.Crypto
{
	public class CryptoClientFactory : ICryptoClientFactory
	{
		private CoinType Coin { get; set; }
		private NetworkType Network { get; set; }

		private static readonly Dictionary<(CoinType coinType, NetworkType networkType), CryptoClient> clients = new() { };

		private string? GetConfig(string configKey)
		{
			return Program.Configuration?[$"CRYPTONODES_{Coin}_{Network}_{configKey}"];
		}

		public CryptoClient GetClient(CoinType coinType, NetworkType networkType)
		{
			CryptoClient client;
			return !clients.TryGetValue((coinType, networkType), out client!) ? CreateClient(coinType, networkType) : client;
		}

		private CryptoClient CreateClient(CoinType coinType, NetworkType networkType)
		{
			Coin = coinType;
			Network = networkType;

			var serverConfig = GetConfig("Server");
			if (serverConfig == null)
			{
				throw new Exception();
			}

			var server = new Uri(serverConfig);
			var credentials = new NetworkCredential(GetConfig("Username"), GetConfig("Password"));
			var network = Network switch
			{
				NetworkType.MainNet => NBitcoin.Network.Main,
				NetworkType.TestNet => NBitcoin.Network.TestNet,
				_ => throw new NotImplementedException()
			};
			var bip44CoinType = Network switch
			{
				NetworkType.MainNet => coinType switch
				{
					CoinType.Bitcoin => 0,
					_ => throw new NotImplementedException()
				},
				NetworkType.TestNet => 1,
				_ => throw new NotImplementedException()
			};

			return new CryptoClient(server, credentials, network, bip44CoinType);
		}
	}
}