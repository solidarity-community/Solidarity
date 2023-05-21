namespace Solidarity.Application.Authentication;

public class AddOrUpdateAuthenticationMethod<T> where T : AuthenticationMethod
{
	private readonly IDatabase _database;
	private readonly GetAccount _getAccount;
	public AddOrUpdateAuthenticationMethod(IDatabase database, GetAccount getAccount) => (_database, _getAccount) = (database, getAccount);

	public async Task<T> Execute(T authentication)
	{
		var account = await _getAccount.Execute(authentication.AccountId);
		authentication.Encrypt();
		if (account.AuthenticationMethods.SingleOrDefault(auth => auth is T) is not T existingAuthentication)
		{
			_database.AuthenticationMethods.Add(authentication);
			await _database.CommitChangesAsync();
			return authentication.WithoutData();
		}
		else
		{
			existingAuthentication.Data = authentication.Data;
			await _database.CommitChangesAsync();
			return existingAuthentication.WithoutData();
		}
	}
}