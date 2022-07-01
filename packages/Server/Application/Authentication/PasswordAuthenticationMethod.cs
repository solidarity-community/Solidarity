namespace Solidarity.Application.Authentication;

public class PasswordAuthenticationMethod : AuthenticationMethod
{
	public override AuthenticationMethodType Type => AuthenticationMethodType.Password;
	public override bool SupportsMultiple => false;
	protected override string GetEncrypted(string data)
	{
		if (data == null)
		{
			return null!;
		}
		HashAlgorithm algorithm = SHA256.Create();
		var hashedArr = algorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
		var sb = new StringBuilder();
		foreach (var b in hashedArr)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}
}