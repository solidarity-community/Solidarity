using Solidarity.Application.Common;
using Solidarity.Application.Helpers;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;

namespace Solidarity.Application.Services
{
	public class AuthenticationService : Service
	{
		private readonly AccountService accountService;

		public AuthenticationService(AccountService accountService, IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService)
		{
			this.accountService = accountService;
		}

		public bool IsAuthenticated()
		{
			return currentUserService.Id is not null && accountService.Exists(user => user.Id == currentUserService.Id);
		}

		public AuthenticationList GetAll()
		{
			return accountService.Get(null).GetAuthenticationList();
		}

		public T AddOrUpdate<T>(T authentication) where T : AuthenticationMethod
		{
			var account = accountService.Get(authentication.AccountId);
			var existingAuthentication = account.GetAuthentication<T>();
			if (existingAuthentication is null)
			{
				// Note that the "Data" property of authentication is not encrypted yet. The setter encrypts it.
				authentication.Data = authentication.Data;
				database.Authentications.Add(authentication);
				account.SetAuthenticationMethod(authentication);
				database.CommitChanges();
				return authentication.WithoutData();
			}
			else
			{
				existingAuthentication.Data = authentication.Data;
				database.CommitChanges();
				return existingAuthentication.WithoutData();
			}
		}

		public string Login<T>(int accountId, string data) where T : AuthenticationMethod
		{
			var account = accountService.Get(accountId);
			var authMethod = account.GetAuthentication<T>();
			return authMethod == null || !authMethod.Authenticate(data)
				? throw new IncorrectCredentialsException()
				: account.IssueAccountAccess(TimeSpan.FromDays(30));
		}

		public void UpdatePassword(string newPassword, string? oldPassword = null)
		{
			var userId = currentUserService.Id ?? throw new NotAuthenticatedException();
			var account = accountService.Get(userId);
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
			var account = accountService.GetByUsername(username);
			var token = Login<PasswordAuthentication>(account.Id, password);
			return token;
		}
	}
}

// private void DeleteInvalid()
// {
//	foreach (var handshake in database.Handshakes.Where(e => e.Expiration <= DateTime.Now))
//	{
//		database.Remove(handshake);
//	}
//	database.SaveChanges();
// }