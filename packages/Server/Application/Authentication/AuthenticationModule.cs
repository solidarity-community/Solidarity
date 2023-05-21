namespace Solidarity.Application.Authentication;

public class AuthenticationModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetAuthenticationMethods>();
		services.AddTransient<UpdatePassword>();
		services.AddTransient<AuthenticatePassword>();
		services.AddTransient<Authenticate<PasswordAuthenticationMethod>>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/authentication",
			[AllowAnonymous] (GetAuthenticationMethods getAuthenticationMethods) => getAuthenticationMethods.Execute()
		);

		endpoints.MapPut("/authentication/password",
			(UpdatePassword updatePassword, string newPassword, string? oldPassword)
				=> updatePassword.Execute(newPassword, oldPassword)
		);

		endpoints.MapGet("/authentication/password",
			[AllowAnonymous] (AuthenticatePassword authenticatePassword, string username, string password)
				=> authenticatePassword.Execute(username, password)
		);
	}
}