namespace Solidarity.Application.Campaigns.Funding;

public sealed class CampaignMediaModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<SaveCampaignMedia>();
		services.AddTransient<DeleteCampaignMedia>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/media/{id}", [AllowAnonymous] (IDatabase database, int id) => database.Get<CampaignMedia>(id));
		var postAndPut = (SaveCampaignMedia save, CampaignMedia media) => save.Execute(media);
		endpoints.MapPost("/api/media", postAndPut);
		endpoints.MapPut("/api/media", postAndPut);
		endpoints.MapDelete("/api/media/{id}", (DeleteCampaignMedia delete, int id) => delete.Execute(id));
	}
}