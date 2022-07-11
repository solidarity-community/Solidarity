namespace Solidarity.Application.Authentication;

public class AuthenticationModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/authentication/check",
			[AllowAnonymous] (AuthenticationService authenticationService) => authenticationService.IsAuthenticated()
		);

		endpoints.MapGet("/authentication",
			[AllowAnonymous] (AuthenticationService authenticationService) => authenticationService.GetAll()
		);

		endpoints.MapPut("/authentication/password",
			(AuthenticationService authenticationService, string newPassword, string? oldPassword)
				=> authenticationService.UpdatePassword(newPassword, oldPassword)
		);

		endpoints.MapGet("/authentication/password",
			[AllowAnonymous] (AuthenticationService authenticationService, string username, string password)
				=> authenticationService.PasswordLogin(username, password)
		);
	}
}