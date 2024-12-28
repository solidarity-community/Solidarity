namespace Solidarity.Application.Auditing;

public sealed class AuditingModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
		=> services.AddTransient<AuditSaveChangesInterceptor>();
}