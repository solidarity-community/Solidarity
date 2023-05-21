namespace Solidarity.Application.Campaigns.Allocation;

public class CampaignAllocationModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<AllocateAllValidatedCampaignFunds>();
		services.AddHostedService<CampaignAllocationBackgroundService>();
	}
}