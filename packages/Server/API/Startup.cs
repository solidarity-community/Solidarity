namespace Solidarity;

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
			app.UseDeveloperExceptionPage()
				.UseCors(builder => builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
				);
		}

		app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
			.UseHttpsRedirection()
			.UseRouting()
			.UseAuthentication()
			.UseAuthorization()
			.ConfigureExceptionHandler()
			.UseEndpoints(endpoints => endpoints.MapControllers())
		.UseSwagger();
	}
}