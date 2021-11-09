namespace Solidarity.Domain.Extensions;

public static class AccountExtentions
{
	public static Account WithoutAuthenticationData(this Account account)
	{
		account.AuthenticationMethods = new();
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