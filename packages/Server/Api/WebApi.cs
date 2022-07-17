namespace Solidarity.Api;

public class WebApi : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddCors();
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
		{
			var secretKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = "Solidarity",
				ValidAudience = "Solidarity",
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
			};
		});
		services.AddAuthorization(options =>
		{
			options.FallbackPolicy = new AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireAuthenticatedUser()
				.Build();
		});
		services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Warning);
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
		application.UseAuthentication();
		application.UseAuthorization();
		application.UseSolidarityExceptionHandler();
	}
}