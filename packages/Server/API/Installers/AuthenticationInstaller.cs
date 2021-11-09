namespace Solidarity.Installers;

public class AuthenticationInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
		{
			var secretKey = Program.Configuration!["JWT_KEY"];
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
	}
}