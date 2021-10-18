using Solidarity.Application.Common;
using Solidarity.Application.Extensions;
using Solidarity.Application.Helpers;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Solidarity.Application.Services
{
	public class AccountService : Service
	{
		private readonly HandshakeService handshakeService;

		public AccountService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService, HandshakeService handshakeService) : base(database, cryptoClientFactory, currentUserService)
		{
			this.handshakeService = handshakeService;
		}

		public Account Get(int? id)
		{
			return database.Accounts.Find(id ?? currentUserService.Id)
				?? throw new EntityNotFoundException("Account Not found");
		}

		public Account GetWithoutAuthentication(int? id)
		{
			return Get(id).WithoutAuthenticationData();
		}

		public Account GetByUsername(string username)
		{
			return database.Accounts.SingleOrDefault(account => account.Username == username)
				?? throw new EntityNotFoundException("Account Not found");
		}

		public bool Exists(Expression<Func<Account, bool>> predicate)
		{
			return database.Accounts.SingleOrDefault(predicate) != null;
		}

		public bool IsUsernameAvailable(string username)
		{
			return database.Accounts.SingleOrDefault(a => a.Username == username.ToLower()) == null;
		}

		public string Create(Account account)
		{
			if (string.IsNullOrWhiteSpace(account.Username) == false && IsUsernameAvailable(account.Username) == false)
			{
				throw new AccountTakenException("This username is not available");
			}

			account.Username = account.Username.ToLower();
			database.Accounts.Add(account);

			database.CommitChanges();

			var token = account.IssueAccountAccess(TimeSpan.FromDays(30));
			return token;
		}

		public Account Update(Account model)
		{
			model.Id = currentUserService.Id ?? throw new Exception("You are not authenticated");
			var account = Get(model.Id);

			if (model.Username != account.Username)
			{
				if (IsUsernameAvailable(model.Username) == false)
				{
					throw new AccountTakenException("This username is not available");
				}
				account.Username = model.Username.ToLower();
			}

			if (model.Identity != null)
			{
				account.Identity ??= new Identity();
				account.Identity.AccountId = account.Id;
				account.Identity.FirstName = model.Identity.FirstName;
				account.Identity.LastName = model.Identity.LastName;
				account.Identity.BirthDate = model.Identity.BirthDate;
			}

			database.CommitChanges();
			return account.WithoutAuthenticationData();
		}

		public string ResetByUsername(string username)
		{
			var account = GetByUsername(username);
			return Reset(account.Id);
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
			database.Handshakes.Add(handshake);
			database.CommitChanges();

			var publicRsaKey = RSA.Create();
			// TODO This fails
			publicRsaKey.ImportRSAPublicKey(Convert.FromBase64String(account.PublicKey), out _);
			// Encrypt Handshake with the public key of the SignDataToString(handshake.Phrase);
			var encryptedHandshake = publicRsaKey.SignDataToString(handshake.Phrase) ?? throw new Exception("Cannot sign data");
			return encryptedHandshake;
		}

		public string Recover(string phrase)
		{
			var handshake = handshakeService.GetByPhrase(phrase);
			var token = handshake.Account.IssueAccountAccess(TimeSpan.FromDays(30));
			handshake.Account.DeleteAuthenticationMethods();
			database.Authentications.RemoveRange(database.Authentications.Where(e => e.AccountId == handshake.Account.Id));
			database.Handshakes.Remove(handshake);
			database.CommitChanges();
			return token;
		}
	}
}