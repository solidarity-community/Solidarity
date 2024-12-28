namespace Solidarity.Application.Campaigns.Allocation;

public sealed class CampaignAllocationModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
		=> services.AddHostedService<CampaignFundAllocator>();
}