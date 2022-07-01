namespace Solidarity;

public abstract class Module
{
	public static List<Module> All => typeof(Module).Assembly
		.GetTypes()
		.Where(p => p.IsClass && p.IsAssignableTo(typeof(Module)) && p.IsAbstract == false)
		.Select(Activator.CreateInstance)
		.Cast<Module>()
		.ToList();

	public virtual void ConfigureServices(IServiceCollection services) { }
	public virtual void ConfigureApplication(WebApplication application) { }
	public virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints) { }
}

public static class ModuleExtensions
{
	public static WebApplicationBuilder InstallSolidarity(this WebApplicationBuilder builder)
	{
		builder.Services.AddServicesOfAllTypes();
		builder.Services.ConfigureSolidarityModulesServices();
		return builder;
	}

	public static WebApplication ConfigureSolidarity(this WebApplication application)
	{
		application.ConfigureSolidarityModulesApplication();
		application.UseEndpoints(endpoints => endpoints.ConfigureSolidarityModulesEndpoints());
		return application;
	}

	private static void ConfigureSolidarityModulesServices(this IServiceCollection services)
		=> Module.All.ForEach(module => module.ConfigureServices(services));

	private static void ConfigureSolidarityModulesApplication(this WebApplication application)
		=> Module.All.ForEach(module => module.ConfigureApplication(application));

	private static void ConfigureSolidarityModulesEndpoints(this IEndpointRouteBuilder endpoints)
		=> Module.All.ForEach(module => module.ConfigureEndpoints(endpoints));
}