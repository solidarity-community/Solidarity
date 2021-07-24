using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Solidarity.Application.Common;
using Solidarity.Infrastructure.Persistance;

namespace Solidarity.Installers
{
	public class DatabaseInstaller : IInstaller
	{
		public void Install(IServiceCollection services)
		{
			services.AddDbContext<IDatabase, DatabaseContext>(options =>
				options.UseSqlServer(Program.Configuration?["Database:ConnectionString"], x => x.UseNetTopologySuite()));
		}
	}
}