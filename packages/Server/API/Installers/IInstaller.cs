using Microsoft.Extensions.DependencyInjection;

namespace Solidarity.Installers
{
	internal interface IInstaller
	{
		void Install(IServiceCollection services);
	}
}