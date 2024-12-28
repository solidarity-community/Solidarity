namespace Solidarity.Pages;

public sealed class ClientModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
		=> services.AddRazorPages(options => options.Conventions.AllowAnonymousToFolder("/"));

	public override void ConfigureApplication(WebApplication application)
	{
		application.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
			ServeUnknownFileTypes = true,
		});
		application.MapRazorPages();
	}
}