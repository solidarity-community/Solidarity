namespace Solidarity.Application.Campaigns.Funding;

public class CampaignMediaModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<SaveCampaignMedia>();
		services.AddTransient<DeleteCampaignMedia>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/media/{id}",
			[AllowAnonymous] (IDatabase database, int id) => database.CampaignMedia.Get(id)
		);

		var postAndPut = (SaveCampaignMedia saveCampaignMedia, CampaignMedia media) => saveCampaignMedia.Execute(media);
		endpoints.MapPost("/media", postAndPut);
		endpoints.MapPut("/media", postAndPut);

		endpoints.MapDelete("/media/{id}", (DeleteCampaignMedia deleteCampaignMedia, int id) => deleteCampaignMedia.Execute(id));
	}
}