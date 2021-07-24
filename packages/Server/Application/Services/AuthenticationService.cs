// private void DeleteInvalid()
// {
// 	foreach (var handshake in database.Handshakes.Where(e => e.Expiration <= DateTime.Now))
// 	{
// 		database.Remove(handshake);
// 	}

// 	database.SaveChanges();
// }

using Solidarity.Application.Common;
using Solidarity.Application.Helpers;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;

namespace Solidarity.Application.Services
{
	public class AuthenticationService : Service<AuthenticationMethod>
	{
		private AccountService AccountService { get; }

		public AuthenticationService(AccountService accountService, IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService)
		{
			AccountService = accountService;
		}

		public bool IsAuthenticated()
		{
			return CurrentUserService.Id is not null && AccountService.Exists(user => user.Id == CurrentUserService.Id);
		}

		public AuthenticationList GetAll()
		{
			var account = AccountService.Get(null);
			return account.GetAuthenticationList();
		}

		public T AddOrUpdate<T>(T authentication) where T : AuthenticationMethod
		{
			var account = AccountService.Get(authentication.AccountId);
			var existingAuthentication = account.GetAuthentication<T>();
			if (existingAuthentication is null)
			{
				// Note that the "Data" property of authentication is not encrypted yet. The setter encrypts it.
				authentication.Data = authentication.Data;
				Database.Authentications.Add(authentication);
				account.SetAuthenticationMethod(authentication);
				Database.CommitChanges();
				return authentication.WithoutData();
			}
			else
			{
				existingAuthentication.Data = authentication.Data;
				Database.CommitChanges();
				return existingAuthentication.WithoutData();
			}
		}

		public string Login<T>(int accountId, string data) where T : AuthenticationMethod
		{
			var account = AccountService.Get(accountId);
			var authMethod = account.GetAuthentication<T>();
			return authMethod == null || !authMethod.Authenticate(data)
				? throw new IncorrectCredentialsException()
				: account.IssueAccountAccess(TimeSpan.FromDays(30));
		}

		public void UpdatePassword(string newPassword, string? oldPassword = null)
		{
			var userId = CurrentUserService.Id ?? throw new NotAuthenticatedException();
			var account = AccountService.Get(userId);
			var existingAuthentication = account.GetAuthentication<PasswordAuthentication>();

			if (string.IsNullOrEmpty(oldPassword) == false && existingAuthentication?.Authenticate(oldPassword) == false)
			{
				throw new IncorrectCredentialsException();
			}

			AddOrUpdate(new PasswordAuthentication
			{
				AccountId = userId,
				Data = newPassword,
			});
		}

		public string PasswordLogin(string username, string password)
		{
			var account = AccountService.GetByUsername(username);
			var token = Login<PasswordAuthentication>(account.Id, password);
			return token;
		}
	}
}