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
		services.AddTransient<SaveCampaign>();
		services.AddTransient<DeleteCampaign>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/campaign",
			[AllowAnonymous] (IDatabase database) => database.Campaigns.GetAll()
		);

		endpoints.MapGet("/campaign/{id}",
			[AllowAnonymous] (IDatabase database, int id) => database.Campaigns.Get(id)
		);

		endpoints.MapPost("/campaign",
			(SaveCampaign saveCampaign, Campaign campaign) => saveCampaign.Create(campaign)
		);

		endpoints.MapPut("/campaign/{id}",
			(SaveCampaign saveCampaign, Campaign campaign) => saveCampaign.Update(campaign)
		);

		endpoints.MapDelete("/campaign/{id}",
			(DeleteCampaign deleteCampaign, int id) => deleteCampaign.Execute(id)
		);
	}
}