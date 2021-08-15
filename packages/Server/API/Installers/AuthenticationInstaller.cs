using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Solidarity.Installers
{
	public class AuthenticationInstaller : IInstaller
	{
		public void Install(IServiceCollection services)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				// Set validation parameters
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = "Solidarity",
					ValidAudience = "Solidarity",
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.Configuration!["Jwt:SecretKey"])),
				};
			});
		}
	}
}