namespace Solidarity.API.Extensions;

public static class ExceptionMiddlewareExtensions
{
	public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder application)
	{
		application.UseExceptionHandler(error => error.Run(async context =>
		{
			Exception exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
			context.Response.StatusCode = exception switch
			{
				EntityNotFoundException<object> => (int)HttpStatusCode.NotFound,
				AccountTakenException => (int)HttpStatusCode.Conflict,
				IncorrectCredentialsException or NotAuthenticatedException or AuthenticationFailedException => (int)HttpStatusCode.Unauthorized,
				_ => (int)HttpStatusCode.InternalServerError
			};
				// TODO the output does not contain the error message
				await context.Response.WriteAsync(exception.Message);
		}));
		return application;
	}
}