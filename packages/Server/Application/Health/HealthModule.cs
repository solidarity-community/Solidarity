namespace Solidarity.Application.Health;

public sealed class HealthModule : Module
{
	public override void ConfigureApplication(WebApplication application)
	{
		application.MapHealthChecks("/api/health", new()
		{
			AllowCachingResponses = true,
			ResponseWriter = async (context, report) =>
			{
				context.Response.ContentType = "application/json";
				var result = JsonSerializer.Serialize(new Health(report), Api.JsonSerializerOptions.Default);
				await context.Response.WriteAsync(result);
			}
		}).AllowAnonymous();
	}
}