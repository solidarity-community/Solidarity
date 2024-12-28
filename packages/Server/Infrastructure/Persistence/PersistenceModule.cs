namespace Solidarity.Infrastructure.Persistence;

public sealed class PersistenceModule : Module
{
	public override void ConfigureServices(IServiceCollection services) => services
		.AddDbContext<IDatabase, DatabaseContext>(
			(provider, options) => options
				.UseSqlServer(DatabaseContext.connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).UseNetTopologySuite().EnableRetryOnFailure()),
			// .AddInterceptors(
			// 	provider.GetRequiredService<AuditSaveChangesInterceptor>()
			// ),
			ServiceLifetime.Transient,
			ServiceLifetime.Transient
		)
		.AddHealthChecks().AddSqlServer(DatabaseContext.connectionString, name: "SQL Server");

	public override void ConfigureApplication(WebApplication application)
		=> application.Services.GetRequiredService<IDatabase>().Initialize();
}