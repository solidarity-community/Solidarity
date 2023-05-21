namespace Solidarity.Application.Accounts;

public class AccountModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetAuthenticatedAccount>();
		services.AddTransient<CreateAccount>();
		services.AddTransient<IsAccountUsernameAvailable>();
		services.AddTransient<UpdateAccount>();
		services.AddTransient<ResetAccountByUsername>();
		services.AddTransient<RecoverAccount>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/account/authenticated",
			(GetAuthenticatedAccount getAuthenticatedAccount) => getAuthenticatedAccount.Execute()
		);

		endpoints.MapPost("/account",
			[AllowAnonymous] (CreateAccount createAccount, Account account) => createAccount.Execute(account)
		);

		endpoints.MapGet("/account/is-username-available/{username}",
			[AllowAnonymous] (IsAccountUsernameAvailable isAccountUsernameAvailable, string username) => isAccountUsernameAvailable.Execute(username)
		);

		endpoints.MapPut("/account",
			(UpdateAccount updateAccount, Account account) => updateAccount.Execute(account)
		);

		endpoints.MapGet("/account/{username}/reset",
			[AllowAnonymous] (ResetAccountByUsername resetAccountByUsername, string username) => resetAccountByUsername.Execute(username)
		);

		endpoints.MapGet("/account/recover",
			[AllowAnonymous] (RecoverAccount recoverAccount, string phrase) => recoverAccount.Execute(phrase)
		);
	}
}