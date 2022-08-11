namespace Solidarity.Application.HealthChecks;

public class HealthModule : Module
{
	public override void ConfigureApplication(WebApplication application)
	{
		application.MapHealthChecks("/health", new()
		{
			AllowCachingResponses = true,
			ResponseWriter = async (context, report) =>
			{
				context.Response.ContentType = "application/json";
				var result = JsonSerializer.Serialize(
					new Health(report),
					options: new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
				);
				await context.Response.WriteAsync(result);
			}
		}).AllowAnonymous();
	}
}