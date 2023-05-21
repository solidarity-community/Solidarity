namespace Solidarity.Application.Accounts;

public class RecoverAccount
{
	private readonly IDatabase _database;
	public RecoverAccount(IDatabase database) => _database = database;

	public async Task<string> Execute(string phrase)
	{
		foreach (var expiredHandshake in _database.AccountRecoveryHandshakes.Where(e => e.Expiration <= DateTime.Now))
		{
			_database.AccountRecoveryHandshakes.Remove(expiredHandshake);
		}

		await _database.CommitChangesAsync();

		var handshake = await _database.AccountRecoveryHandshakes.FirstOrDefaultAsync(hs => hs.Phrase == phrase)
			?? throw new EntityNotFoundException<AccountRecoveryHandshake>();

		var token = handshake.Account.IssueToken(TimeSpan.FromDays(30));

		handshake.Account.AuthenticationMethods = new();
		_database.AuthenticationMethods.RemoveRange(_database.AuthenticationMethods.Where(e => e.AccountId == handshake.AccountId));
		_database.AccountRecoveryHandshakes.Remove(handshake);

		await _database.CommitChangesAsync();
		return token;
	}
}