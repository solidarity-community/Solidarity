namespace Solidarity.Installers;

public class DatabaseInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddDbContext<IDatabase, DatabaseContext>(options =>
		{
			var server = Environment.GetEnvironmentVariable("DATABASE_SERVER");
			var user = Environment.GetEnvironmentVariable("DATABASE_USER");
			var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");

			var connectionString = server is null || user is null || password is null
				? "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Solidarity;Integrated Security=True;"
				: $"Server={server};Initial Catalog=Solidarity;User ID={user};Password={password};";

			options.UseSqlServer(connectionString, options => options.UseNetTopologySuite());
		});
	}
}