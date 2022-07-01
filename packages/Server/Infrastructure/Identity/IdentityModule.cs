namespace Solidarity.Infrastructure.Identity;
public class IdentityModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddHttpContextAccessor();
	}
}
