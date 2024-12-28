namespace Solidarity.Infrastructure.Identity;

public sealed class IdentityModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.AddScoped<IAuthenticatedAccount, AuthenticatedAccount>();
	}
}
