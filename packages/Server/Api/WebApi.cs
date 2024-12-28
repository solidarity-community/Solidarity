using Serilog;

namespace Solidarity.Api;

public sealed class WebApi : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddCors();
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = "Solidarity",
				ValidAudience = "Solidarity",
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvironmentVariables.JWT_KEY)),
			};
		});
		services.AddAuthorizationBuilder()
			.SetFallbackPolicy(new AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireAuthenticatedUser()
				.Build());
		Serilog.Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
			.WriteTo.Console(
				theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate,
				applyThemeToRedirectedOutput: true,
				outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] [{SourceContext}]{NewLine}{Message:lj}{NewLine}{Exception}\n"
			)
			// .WriteTo.MSSqlServer(
			// 	connectionString: DatabaseContext.connectionString,
			// 	sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions { TableName = "Logs" }
			// )
			.Enrich.FromLogContext()
			.CreateLogger();
		services.AddSerilog();
		services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => JsonSerializerOptions.Get(options.SerializerOptions));
	}

	public override void ConfigureApplication(WebApplication application)
	{
		application.UseRouting();
		application.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
		application.UseHttpsRedirection();
		application.UseAuthentication();
		application.UseAuthorization();
		application.UseSolidarityExceptionHandler();
	}
}