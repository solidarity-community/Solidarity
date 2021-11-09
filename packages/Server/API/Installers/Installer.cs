namespace Solidarity.Installers;

public static class Installer
{
	public static IServiceCollection InstallSolidarity(this IServiceCollection services)
	{
		new AuthenticationInstaller().Install(services);
		new CryptoClientInstaller().Install(services);
		new DatabaseInstaller().Install(services);
		new OpenApiInstaller().Install(services);
		new UserServiceInstaller().Install(services);
		new ServiceInstaller().Install(services);
		return services;
	}
}