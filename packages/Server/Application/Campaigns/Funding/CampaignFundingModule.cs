namespace Solidarity.Application.Campaigns.Funding;

public class CampaignFundingModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetCampaignBalance>();
		services.AddTransient<GetCampaignBalanceShare>();
		services.AddTransient<GetCampaignDonationData>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/campaign/{id}/balance",
			[AllowAnonymous] (GetCampaignBalance getCampaignBalance, int id) => getCampaignBalance.Execute(id)
		);

		endpoints.MapGet("/campaign/{id}/share",
			[AllowAnonymous] (GetCampaignBalanceShare getCampaignBalanceShare, int id) => getCampaignBalanceShare.Execute(id)
		);

		endpoints.MapGet("/campaign/{id}/donation-data",
			[AllowAnonymous] (GetCampaignDonationData getCampaignDonationData, int id) => getCampaignDonationData.Execute(id)
		);
	}
}