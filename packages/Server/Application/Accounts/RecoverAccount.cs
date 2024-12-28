namespace Solidarity.Application.Accounts;

public sealed class RecoverAccount(IDatabase database)
{
	public async Task<string> Execute(string phrase)
	{
		foreach (var expiredHandshake in database.AccountRecoveryHandshakes.Where(e => e.Expiration <= DateTime.Now))
		{
			database.AccountRecoveryHandshakes.Remove(expiredHandshake);
		}

		await database.CommitChanges();

		var handshake = await database.Get<AccountRecoveryHandshake>(hs => hs.Phrase == phrase);

		var token = handshake.Account.IssueToken(TimeSpan.FromDays(30));

		handshake.Account.Password = null;
		database.AccountRecoveryHandshakes.Remove(handshake);

		await database.CommitChanges();
		return token;
	}
}