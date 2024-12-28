namespace Solidarity.Application.Campaigns;

public sealed class CampaignModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/campaign", [AllowAnonymous] (IDatabase database) => database.GetAll<Campaign>());
		endpoints.MapGet("/api/campaign/{id}", [AllowAnonymous] (IDatabase database, int id) => database.Get<Campaign>(id));

		var save = (Campaign campaign, IDatabase database, IAuthenticatedAccount authenticatedAccount, IPaymentMethodProvider paymentMethodProvider) => campaign.Save(database, authenticatedAccount, paymentMethodProvider);
		endpoints.MapPost("/api/campaign", save);
		endpoints.MapPut("/api/campaign", save);

		endpoints.MapDelete("/api/campaign/{id}", async (int id, IDatabase database, IPaymentMethodProvider paymentMethodProvider) => (await database.Get<Campaign>(id)).Delete(database, paymentMethodProvider));
	}
}