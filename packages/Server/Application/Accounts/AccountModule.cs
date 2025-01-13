namespace Solidarity.Application.Accounts;

public sealed class AccountModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetAuthenticatedAccount>();
		services.AddTransient<IsAccountAuthenticated>();
		services.AddTransient<GetAccountByUsername>();
		services.AddTransient<CreateAccount>();
		services.AddTransient<IsAccountUsernameAvailable>();
		services.AddTransient<UpdateAccount>();
		services.AddTransient<AuthenticateAccount>();
		services.AddTransient<ResetAccountByUsername>();
		services.AddTransient<RecoverAccount>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/account/authenticated", (GetAuthenticatedAccount getAuthenticatedAccount) => getAuthenticatedAccount.Execute());
		endpoints.MapGet("/api/account/is-authenticated", [AllowAnonymous] (IsAccountAuthenticated isAuthenticated) => isAuthenticated.Execute());
		endpoints.MapPost("/api/account", [AllowAnonymous] (CreateAccount create, Account account) => create.Execute(account));
		endpoints.MapGet("/api/account/is-username-available/{username}", [AllowAnonymous] (IsAccountUsernameAvailable isAccountUsernameAvailable, string username) => isAccountUsernameAvailable.Execute(username));
		endpoints.MapPut("/api/account", (UpdateAccount update, Account account) => update.Execute(account));
		endpoints.MapGet("/api/account/{username}/reset", [AllowAnonymous] (ResetAccountByUsername reset, string username) => reset.Execute(username));
		endpoints.MapGet("/api/account/recover", [AllowAnonymous] (RecoverAccount recover, string phrase) => recover.Execute(phrase));
		endpoints.MapPost("/api/account/authenticate", [AllowAnonymous] (AuthenticateAccount authenticate, [FromBody] AuthenticateAccount.Parameters parameters) => authenticate.Execute(parameters));
	}
}