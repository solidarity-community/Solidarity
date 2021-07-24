using Solidarity.Domain.Models;
using System.Collections.Generic;

namespace Solidarity.Domain.Extensions
{
	public static class AccountExtentions
	{
		public static Account WithoutAuthenticationData(this Account account)
		{
			account.GetAuthentications().WithoutData();
			return account;
		}

		public static IEnumerable<Account> WithoutAuthenticationData(this IEnumerable<Account> accounts)
		{
			foreach (var account in accounts)
			{
				account.WithoutAuthenticationData();
			}

			return accounts;
		}
	}
}