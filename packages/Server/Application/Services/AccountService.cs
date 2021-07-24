using Solidarity.Application.Common;
using Solidarity.Application.Extensions;
using Solidarity.Application.Helpers;
using Solidarity.Core.Application;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Solidarity.Application.Services
{
	public class AccountService : Service
	{
		private HandshakeService HandshakeService { get; }

		public AccountService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService, HandshakeService handshakeService) : base(database, cryptoClientFactory, currentUserService)
		{
			HandshakeService = handshakeService;
		}

		public Account Get(int? id)
		{
			return Database.Accounts.Find(id ?? CurrentUserService.Id)
				?? throw new EntityNotFoundException("Account Not found");
		}

		public Account GetWithoutAuthentication(int? id)
		{
			return Get(id).WithoutAuthenticationData();
		}

		public Account GetByUsername(string username)
		{
			return Database.Accounts.SingleOrDefault(account => account.Username == username)
				?? throw new EntityNotFoundException("Account Not found");
		}

		public bool Exists(Expression<Func<Account, bool>> predicate)
		{
			return Database.Accounts.SingleOrDefault(predicate) != null;
		}

		public string Create(Account account)
		{
			if (!IsUsernameAvailable(account.Username))
			{
				throw new AccountTakenException("This username is not available");
			}

			account.Username = account.Username.ToLower();
			Database.Accounts.Add(account);

			Database.CommitChanges();

			var token = account.IssueAccountAccess(TimeSpan.FromDays(30));
			return token;
		}

		public bool IsUsernameAvailable(string username)
		{
			return Database.Accounts.SingleOrDefault(a => a.Username == username.ToLower()) == null;
		}

		public Account Update(Account model)
		{
			model.Id = CurrentUserService.Id ?? throw new Exception("You are not logged in");
			var account = Get(model.Id);

			if (model.Username != account.Username && !IsUsernameAvailable(model.Username))
			{
				throw new AccountTakenException("This username is not available");
			}

			account.Username = model.Username.ToLower();
			if (model.Identity != null)
			{
				if (account.Identity == null)
				{
					account.Identity = new Identity()
					{
						AccountId = account.Id,
						FirstName = model.Identity.FirstName,
						LastName = model.Identity.LastName,
						BirthDate = model.Identity.BirthDate
					};
				}
				else
				{
					account.Identity.AccountId = account.Id;
					account.Identity.FirstName = model.Identity.FirstName;
					account.Identity.LastName = model.Identity.LastName;
					account.Identity.BirthDate = model.Identity.BirthDate;
				}
			}

			Database.CommitChanges();
			return account.WithoutAuthenticationData();
		}

		public string Reset(int id)
		{
			var account = Get(id);

			var handshake = new Handshake
			{
				AccountId = account.Id,
				Phrase = RandomStringGenerator.Generate(64),
				Expiration = DateTime.Now.AddMinutes(10)
			};
			Database.Handshakes.Add(handshake);
			Database.CommitChanges();

			// Encrypt Handshake with the public key of the accountoString(handshake.Phrase);
			var encryptedHandshake = account.PublicRSAKey?.SignDataToString(handshake.Phrase) ?? throw new Exception("Cannot sign data");
			return encryptedHandshake;
		}

		public string Recover(string phrase)
		{
			var handshake = HandshakeService.GetByPhrase(phrase);
			var token = handshake.Account.IssueAccountAccess(TimeSpan.FromDays(30));
			handshake.Account.DeleteAuthenticationMethods();
			Database.Authentications.RemoveRange(Database.Authentications.Where(e => e.AccountId == handshake.Account.Id));
			Database.Handshakes.Remove(handshake);
			Database.CommitChanges();
			return token;
		}
	}
}