namespace Solidarity.Infrastructure.Persistence;

public class PersistenceModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		var server = Environment.GetEnvironmentVariable("DATABASE_SERVER");
		var user = Environment.GetEnvironmentVariable("DATABASE_USER");
		var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");

		var connectionString = server is null || user is null || password is null
			? "Data Source=localhost\\SQLEXPRESS;Database=Solidarity;Integrated Security=True;"
			: $"Server={server};User ID={user};Database=Solidarity;Password={password};";

		services.AddHealthChecks().AddSqlServer(connectionString, name: "Database");

		services.AddDbContext<IDatabase, DatabaseContext>(options =>
		{
			options.UseSqlServer(connectionString, options => options
				.EnableRetryOnFailure(10)
				.CommandTimeout(60)
				.UseNetTopologySuite()
			);
		});
	}

	public override void ConfigureApplication(WebApplication application)
	{
		application.Services.GetRequiredService<IDatabase>().Initialize();
	}
}