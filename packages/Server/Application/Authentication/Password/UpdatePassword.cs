namespace Solidarity.Application.Authentication.Password;

public class UpdatePassword
{
	private readonly ICurrentUserService _currentUserService;
	private readonly IDatabase _database;
	private readonly AddOrUpdateAuthenticationMethod<PasswordAuthenticationMethod> _addOrUpdateAuthenticationMethod;
	public UpdatePassword(ICurrentUserService currentUserService, IDatabase database, AddOrUpdateAuthenticationMethod<PasswordAuthenticationMethod> addOrUpdateAuthenticationMethod)
		=> (_currentUserService, _database, _addOrUpdateAuthenticationMethod) = (currentUserService, database, addOrUpdateAuthenticationMethod);

	public async Task Execute(string newPassword, string? oldPassword = null)
	{
		var accountId = _currentUserService.Id.ThrowIfNull("Not authenticated").Value;
		var existingAuthentication = _database.AuthenticationMethods.FirstOrDefault(am => am is PasswordAuthenticationMethod && am.AccountId == accountId);

		(string.IsNullOrEmpty(oldPassword) == false && existingAuthentication?.Authenticate(oldPassword) == false).Throw("Incorrect credentials").IfTrue();

		await _addOrUpdateAuthenticationMethod.Execute(new PasswordAuthenticationMethod
		{
			AccountId = accountId,
			Data = newPassword,
		});
	}
}