namespace Solidarity.Application.Authentication;

public class PasswordAuthenticationMethod : AuthenticationMethod
{
	public override AuthenticationMethodType Type => AuthenticationMethodType.Password;
	public override bool SupportsMultiple => false;
	protected override string GetEncrypted(string data)
	{
		var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
		var sb = new StringBuilder();
		foreach (var b in hashedBytes)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}
}