namespace Solidarity.Api;

public class WebApi : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddCors();
		services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
		{
			options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			options.SerializerOptions.Converters.Add(new DateTimeConverter());
		});
	}

	public override void ConfigureApplication(WebApplication application)
	{
		application.UseRouting();
		application.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
		application.UseHttpsRedirection();
		application.UseSolidarityExceptionHandler();
	}
}