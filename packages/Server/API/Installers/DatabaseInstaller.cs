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
				var server = Program.Configuration?["Database:Server"];
				var user = Program.Configuration?["Database:User"];
				var password = Program.Configuration?["Database:Password"];
				if (server is null || user is null || password is null)
				{
					throw new System.Exception("Environment variables 'Database:Server', 'Database:User' and 'Database:Password' are necessary to establish a database connection.");
				}
				var connectionString = $"Server={server};Initial Catalog=Solidarity;User ID={user};Password={password};";
				options.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
			});
		}
	}
}