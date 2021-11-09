namespace Solidarity.Installers;

public class ServiceInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddTransient<AccountService>();
		services.AddTransient<IdentityService>();
		services.AddTransient<AuthenticationService>();
		services.AddTransient<CampaignService>();
		services.AddTransient<HandshakeService>();
	}
}