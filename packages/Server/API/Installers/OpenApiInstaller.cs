using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Solidarity.Installers
{
	public class OpenApiInstaller : IInstaller
	{
		public void Install(IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Solidarity", Version = "1" });
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Scheme = "Bearer", BearerFormat = "JWT", Type = SecuritySchemeType.Http, });
				options.AddServer(new OpenApiServer { Url = "http://localhost:5001", Description = "Development Server" });
				options.AddServer(new OpenApiServer { Url = "http://api.solidarity.community", Description = "Production Server" });
			});
		}
	}
}