namespace Solidarity.Application.Authentication;

public class Authenticate<T> where T : AuthenticationMethod
{
	private readonly IDatabase _database;
	private readonly GetAccount _getAccount;
	public Authenticate(IDatabase database, GetAccount getAccount) => (_database, _getAccount) = (database, getAccount);

	public async Task<string> Execute(int accountId, string data)
	{
		(_database.AuthenticationMethods.SingleOrDefault(am => am is T && am.AccountId == accountId) is not T authMethod || !authMethod.Authenticate(data))
			.Throw("Incorrect credentials").IfTrue();
		return (await _getAccount.Execute(accountId)).IssueToken(TimeSpan.FromDays(30));
	}
}