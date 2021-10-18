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
			{
				var server = Program.Configuration?["DATABASE_SERVER"];
				var user = Program.Configuration?["DATABASE_USER"];
				var password = Program.Configuration?["DATABASE_PASSWORD"];
				if (server is null || user is null || password is null)
				{
					throw new System.Exception("Environment variables 'DATABASE_SERVER', 'DATABASE_USER' and 'DATABASE_PASSWORD' are necessary to establish a database connection.");
				}
				var connectionString = $"Server={server};Initial Catalog=Solidarity;User ID={user};Password={password};";
				options.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
			});
		}
	}
}