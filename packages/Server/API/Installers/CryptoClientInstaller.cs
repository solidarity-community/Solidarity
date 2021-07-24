using Microsoft.Extensions.DependencyInjection;
using Solidarity.Application.Common;
using Solidarity.Infrastructure.Crypto;

namespace Solidarity.Installers
{
	public class CryptoClientInstaller : IInstaller
	{
		public void Install(IServiceCollection services)
		{
			services.AddScoped<ICryptoClientFactory, CryptoClientFactory>();
		}
	}
}