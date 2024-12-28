namespace Solidarity.Api;

public static class ExceptionHandlerExtension
{
	public static void UseSolidarityExceptionHandler(this WebApplication application)
	{
		application.UseExceptionHandler(builder => builder.Run(async context =>
		{
			var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
			var httpStatusCodeAttribute = exception?.GetType().GetCustomAttributes(typeof(MapToHttpStatusCodeAttribute), true).FirstOrDefault() as MapToHttpStatusCodeAttribute;
			context.Response.StatusCode = (int)(httpStatusCodeAttribute?.Code ?? HttpStatusCode.InternalServerError);
			await context.Response.WriteAsJsonAsync(new
			{
				Status = context.Response.StatusCode,
				Title = exception?.Message ?? "Unknown error",
				TraceId = context.TraceIdentifier,
				Type = context.GetType().Name,
			});
		}));
	}
}