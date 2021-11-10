namespace Solidarity.Domain.Models;

public class PasswordAuthentication : AuthenticationMethod
{
	public override AuthenticationMethodType Type => AuthenticationMethodType.Password;
	public override bool SupportsMultiple => false;
	protected override string GetEncrypted(string data) => data.GetSha256Hash();
}