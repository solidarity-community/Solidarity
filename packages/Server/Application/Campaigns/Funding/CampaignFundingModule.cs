namespace Solidarity.Application.Campaigns.Funding;

public sealed class CampaignFundingModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetCampaignBalanceShare>();
		services.AddTransient<GetCampaignDonationData>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/campaign/{id}/balance", [AllowAnonymous] async (IDatabase database, IPaymentMethodProvider paymentMethodProvider, int id) => await (await database.Get<Campaign>(id)).GetTotalBalance(paymentMethodProvider));
		endpoints.MapGet("/api/campaign/{id}/share", [AllowAnonymous] (GetCampaignBalanceShare getCampaignBalanceShare, int id) => getCampaignBalanceShare.Execute(id));
		endpoints.MapGet("/api/campaign/{id}/donation-data", [AllowAnonymous] (GetCampaignDonationData getCampaignDonationData, int id) => getCampaignDonationData.Execute(id));
	}
}