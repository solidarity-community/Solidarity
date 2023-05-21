namespace Solidarity.Application.Accounts;

public class ResetAccountByUsername
{
	private readonly IDatabase _database;
	private readonly GetAccountByUsername _getAccountByUsername;
	public ResetAccountByUsername(IDatabase database, GetAccountByUsername getAccountByUsername) => (_database, _getAccountByUsername) = (database, getAccountByUsername);

	public async Task<string> Execute(string username)
	{
		var account = await _getAccountByUsername.Execute(username);

		var phrase = RandomStringGenerator.Generate(64);

		var publicRsaKey = RSA.Create();
		publicRsaKey.ImportFromPem(account.PublicKey);
		var encryptedPhrase = Convert.ToBase64String(publicRsaKey.Encrypt(Encoding.ASCII.GetBytes(phrase), RSAEncryptionPadding.Pkcs1));

		_database.AccountRecoveryHandshakes.Add(new()
		{
			AccountId = account.Id,
			Phrase = phrase,
		});
		await _database.CommitChangesAsync();

		return encryptedPhrase;
	}
}