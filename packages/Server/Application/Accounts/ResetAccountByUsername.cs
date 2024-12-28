namespace Solidarity.Application.Accounts;

public sealed class ResetAccountByUsername(IDatabase database, GetAccountByUsername getAccountByUsername)
{
	public async Task<string> Execute(string username)
	{
		var account = await getAccountByUsername.Execute(username);

		var phrase = RandomStringGenerator.Generate(64);

		var publicRsaKey = RSA.Create();
		publicRsaKey.ImportFromPem(account.PublicKey);
		var encryptedPhrase = Convert.ToBase64String(publicRsaKey.Encrypt(Encoding.ASCII.GetBytes(phrase), RSAEncryptionPadding.Pkcs1));

		database.AccountRecoveryHandshakes.Add(new()
		{
			AccountId = account.Id,
			Phrase = phrase,
		});
		await database.CommitChanges();

		return encryptedPhrase;
	}
}