using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Solidarity.Domain.Exceptions;
using System.Net;
using System.Threading.Tasks;

namespace Solidarity.API.Extensions
{
	public static class ExceptionMiddlewareExtensions
	{
		public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder application)
		{
			application.UseExceptionHandler(error => error.Run(context =>
			{
				var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
				context.Response.StatusCode = exception switch
				{
					EntityNotFoundException => (int)HttpStatusCode.NotFound,
					AccountTakenException => (int)HttpStatusCode.Conflict,
					IncorrectCredentialsException or NotAuthenticatedException or AuthenticationFailedException => (int)HttpStatusCode.Unauthorized,
					_ => (int)HttpStatusCode.InternalServerError
				};
				return Task.CompletedTask;
			}));
			return application;
		}
	}
}