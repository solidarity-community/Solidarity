﻿namespace Solidarity.Installers;

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
				? "Data Source=localhost\\SQLEXPRESS;Database=Solidarity;Integrated Security=True;"
				: $"Server={server};User ID={user};Database=Solidarity;Password={password};";

			options.UseSqlServer(connectionString, options => options
				.EnableRetryOnFailure(10)
				.CommandTimeout(60)
				.UseNetTopologySuite()
			);
		});
	}
}