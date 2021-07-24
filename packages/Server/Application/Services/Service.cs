using NBitcoin;
using Solidarity.Application.Common;
using Solidarity.Core.Application;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Solidarity.Application.Services
{
	public abstract class Service
	{
		protected IDatabase Database { get; }
		protected ICryptoClientFactory CryptoClientFactory { get; }
		protected ICurrentUserService CurrentUserService { get; }
		protected ExtKey CryptoPrivateKey { get; }

		public Service(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService)
		{
			Database = database;
			CryptoClientFactory = cryptoClientFactory;
			CurrentUserService = currentUserService;
			CryptoPrivateKey = new Mnemonic(GetMnemonic().Mnemonic, Wordlist.English).DeriveExtKey();
		}

		private CryptoMnemonic GetMnemonic()
		{
			if (Database.CryptoMnemonics.Any())
			{
				return Database.CryptoMnemonics.First();
			}
			else
			{
				var mnemonics = new CryptoMnemonic(new Mnemonic(Wordlist.English, WordCount.TwentyFour).ToString());
				Database.CryptoMnemonics.Add(mnemonics);
				Database.CommitChanges();
				return mnemonics;
			}
		}
	}
}