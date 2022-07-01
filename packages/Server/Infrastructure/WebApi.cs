namespace Solidarity.Infrastructure;

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
		application.UseExceptionHandler(error => error.Run(async context =>
		{
			var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
			context.Response.StatusCode = (int)(exception?.GetType().GetCustomAttribute<MapToHttpStatusCodeAttribute>()?.Code ?? HttpStatusCode.InternalServerError);
			await context.Response.WriteAsJsonAsync(new
			{
				Status = context.Response.StatusCode,
				Title = exception?.Message ?? "Unknown error",
				TraceId = context.TraceIdentifier,
				Type = exception?.GetType().Name,
			});
		}));
	}
}


public class DateTimeConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetDateTime().ToUniversalTime();

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToUniversalTime());
}