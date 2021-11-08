using Solidarity.Application.Common;
using Solidarity.Application.Helpers;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
			return currentUserService.Id is int id
				&& accountService.Exists(id);
		}

		public Dictionary<AuthenticationMethodType, bool> GetAll()
		{
			var availableAuthenticationMethods = database.AuthenticationMethods
				.Where(am => am.AccountId == currentUserService.Id)
				.Select(am => am.Type);

			return new Dictionary<AuthenticationMethodType, bool>
			{
				{ AuthenticationMethodType.Password, availableAuthenticationMethods.Contains(AuthenticationMethodType.Password) },
			};
		}

		public T AddOrUpdate<T>(T authentication) where T : AuthenticationMethod
		{
			var account = accountService.Get(authentication.AccountId);
			var existingAuthentication = account.AuthenticationMethods.SingleOrDefault(auth => auth is T) as T;
			authentication.Encrypt();
			if (existingAuthentication is null)
			{
				database.AuthenticationMethods.Add(authentication);
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
			var authMethod = database.AuthenticationMethods.SingleOrDefault(am => am is T && am.AccountId == accountId) as T;
			return authMethod == null || !authMethod.Authenticate(data)
				? throw new IncorrectCredentialsException()
				: accountService.Get(accountId).IssueToken(TimeSpan.FromDays(30));
		}

		public void UpdatePassword(string newPassword, string? oldPassword = null)
		{
			var accountId = currentUserService.Id ?? throw new NotAuthenticatedException();
			var existingAuthentication = database.AuthenticationMethods.FirstOrDefault(am => am is PasswordAuthentication && am.AccountId == accountId);

			if (string.IsNullOrEmpty(oldPassword) == false && existingAuthentication?.Authenticate(oldPassword) == false)
			{
				throw new IncorrectCredentialsException();
			}

			AddOrUpdate(new PasswordAuthentication
			{
				AccountId = accountId,
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