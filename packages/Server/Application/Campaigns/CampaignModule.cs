namespace Solidarity.Application.Campaigns;

public class CampaignModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
		{
			var geometryFactory = new GeometryFactoryEx(precisionModel: new PrecisionModel(), srid: 4326) { OrientationOfExteriorRing = LinearRingOrientation.CCW };
			options.SerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory(geometryFactory));
		});
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/campaign",
			[AllowAnonymous] (CampaignService campaignService) => campaignService.GetAll()
		);

		endpoints.MapGet("/campaign/{id}",
			[AllowAnonymous] (CampaignService campaignService, int id) => campaignService.Get(id)
		);

		endpoints.MapGet("/campaign/{id}/balance",
			[AllowAnonymous] (CampaignService campaignService, int id) => campaignService.GetBalance(id)
		);

		endpoints.MapGet("/campaign/{id}/share",
			[AllowAnonymous] (CampaignService campaignService, int id) => campaignService.GetShare(id)
		);

		endpoints.MapGet("/campaign/{id}/donation-data",
			[AllowAnonymous] (CampaignService campaignService, int id) => campaignService.GetDonationData(id)
		);

		endpoints.MapPost("/campaign/{id}/allocate",
			(CampaignService campaignService, int id, Dictionary<string, string> destinationByPaymentMethodIdentifier)
				=> campaignService.Allocate(id, destinationByPaymentMethodIdentifier)
		);

		endpoints.MapPost("/campaign/{id}/declare-allocation-phase",
			(CampaignService campaignService, int id) => campaignService.DeclareAllocationPhase(id)
		);

		endpoints.MapPost("/campaign",
			(CampaignService campaignService, Campaign campaign) => campaignService.Create(campaign)
		);

		endpoints.MapPut("/campaign/{id}",
			(CampaignService campaignService, Campaign campaign) => campaignService.Update(campaign)
		);

		endpoints.MapDelete("/campaign/{id}",
			(CampaignService campaignService, int id) => campaignService.Delete(id)
		);

		endpoints.MapGet("/campaign/{id}/vote",
			(CampaignService campaignService, int id) => campaignService.GetVote(id)
		);

		endpoints.MapPost("/campaign/{id}/vote",
			(CampaignService campaignService, int id, [FromBody] bool vote) => campaignService.Vote(id, vote)
		);

		endpoints.MapGet("/media/{id}",
			[AllowAnonymous] (CampaignMediaService campaignMediaService, int id) => campaignMediaService.Get(id)
		);

		var postAndPut = (CampaignMediaService campaignMediaService, CampaignMedia media) => campaignMediaService.CreateOrUpdate(media);
		endpoints.MapPost("/media", postAndPut);
		endpoints.MapPut("/media", postAndPut);

		endpoints.MapDelete("/media/{id}", (CampaignMediaService campaignMediaService, int id) => campaignMediaService.Delete(id));
	}
}