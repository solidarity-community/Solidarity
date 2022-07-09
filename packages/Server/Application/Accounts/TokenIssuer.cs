namespace Solidarity.Application.Accounts;

public static class TokenIssuer
{
	private const string Issuer = "Solidarity";
	private const string Audience = "Solidarity";
	private static readonly string secretKey = Environment.GetEnvironmentVariable("JWT_KEY")!;

	public static string IssueForAccount(Account account, TimeSpan expiration)
	{
		return Issue(expiration, new() {
			{ JwtRegisteredClaimNames.Sub, account.Id.ToString() },
			{ JwtRegisteredClaimNames.NameId, account.Username }
		});
	}

	public static string Issue(TimeSpan expiration, Dictionary<string, string> valuesByClaimNames)
	{
		valuesByClaimNames.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"));
		var claims = valuesByClaimNames.Select(kvp => new Claim(kvp.Key, kvp.Value));
		return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
			issuer: Issuer,
			audience: Audience,
			claims: claims,
			expires: DateTime.Now.Add(expiration),
			signingCredentials: new(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
				SecurityAlgorithms.HmacSha256
			)
		));
	}
}