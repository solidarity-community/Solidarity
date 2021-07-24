using Microsoft.EntityFrameworkCore;
using Solidarity.Domain.Models;

namespace Solidarity.Application.Common
{
	public interface IDatabase
	{
		DbSet<Account> Accounts { get; set; }
		DbSet<Identity> Identities { get; set; }
		DbSet<AuthenticationMethod> Authentications { get; set; }
		DbSet<Handshake> Handshakes { get; set; }
		DbSet<Campaign> Campaigns { get; set; }
		DbSet<Validation> Validations { get; set; }
		DbSet<Vote> Votes { get; set; }
		DbSet<DonationChannel> DonationChannels { get; set; }
		DbSet<CryptoMnemonic> CryptoMnemonics { get; set; }

		DbSet<T> Set<T>() where T : Model;

		void CommitChanges();
		// Task SaveChangesAsync();
	}
}