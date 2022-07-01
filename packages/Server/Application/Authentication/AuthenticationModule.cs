namespace Solidarity.Application.Authentication;

public class AuthenticationModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
		{
			var secretKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = "Solidarity",
				ValidAudience = "Solidarity",
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
			};
		});
		services.AddAuthorization();
	}

	public override void ConfigureApplication(WebApplication application)
	{
		application.UseAuthentication();
		application.UseAuthorization();
	}

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