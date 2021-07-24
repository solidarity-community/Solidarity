using Microsoft.Extensions.DependencyInjection;
using Solidarity.Application.Common;
using Solidarity.Infrastructure.Identity;

namespace Solidarity.Installers
{
	public class UserServiceInstaller : IInstaller
	{
		public void Install(IServiceCollection services) =>
			services.AddTransient<ICurrentUserService, CurrentUserService>();
	}
}