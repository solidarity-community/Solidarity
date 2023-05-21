namespace Solidarity.Application.Authentication.Password;

public class AuthenticatePassword
{
	private readonly GetAccountByUsername _getAccountByUsername;
	private readonly Authenticate<PasswordAuthenticationMethod> _authenticate;
	public AuthenticatePassword(GetAccountByUsername getAccountByUsername, Authenticate<PasswordAuthenticationMethod> authenticate)
		=> (_getAccountByUsername, _authenticate) = (getAccountByUsername, authenticate);

	public async Task<string> Execute(string username, string password)
	{
		var account = await _getAccountByUsername.Execute(username);
		var token = await _authenticate.Execute(account.Id, password);
		return token;
	}
}