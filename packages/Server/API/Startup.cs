using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Solidarity.Installers;

namespace Solidarity
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Program.Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.InstallSolidarity()
				.AddHttpContextAccessor()
				.AddCors()
				.AddControllers()
				.AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
				.UseHttpsRedirection()
				.UseRouting()
				.UseAuthentication()
				.UseAuthorization()
				.UseEndpoints(endpoints => endpoints.MapControllers())
				.UseSwagger();
		}
	}
}