namespace Solidarity.Installers;

public class UserServiceInstaller : IInstaller
{
	public void Install(IServiceCollection services) =>
		services.AddTransient<ICurrentUserService, CurrentUserService>();
}