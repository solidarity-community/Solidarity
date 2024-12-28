namespace Solidarity.Application.Accounts;

public sealed class AuthenticateAccount(IDatabase database)
{
	private static string GetSHA256(string password)
	{
		var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
		var sb = new StringBuilder();
		foreach (var b in hashedBytes)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}

	public record Parameters(string Username, string Password);

	public async Task<string> Execute(Parameters parameters)
	{
		var (username, password) = parameters;
		var account = await database.Accounts.SingleOrDefaultAsync(account => account.Username == username) ?? throw new EntityNotFoundException<Account>(username);

		account.Throw(() => new SecurityException("Invalid username or password"))
			.IfNotEquals(a => a.Username, username)
			.IfNotEquals(a => account.Password!.ToLower(), GetSHA256(password).ToLower());

		var token = account.IssueToken(TimeSpan.FromDays(1));

		await database.CommitChanges();

		return token;
	}
}