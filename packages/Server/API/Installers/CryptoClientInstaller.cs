namespace Solidarity.Installers;

public class CryptoClientInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddScoped<ICryptoClientFactory, CryptoClientFactory>();
	}
}