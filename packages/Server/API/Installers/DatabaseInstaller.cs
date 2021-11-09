namespace Solidarity.Installers;

public class DatabaseInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddDbContext<IDatabase, DatabaseContext>(options =>
		{
			var server = Program.Configuration?["DATABASE_SERVER"];
			var user = Program.Configuration?["DATABASE_USER"];
			var password = Program.Configuration?["DATABASE_PASSWORD"];

			var connectionString = server is null || user is null || password is null
				? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;"
				: $"Server={server};Initial Catalog=Solidarity;User ID={user};Password={password};";

			options.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
		});
	}
}