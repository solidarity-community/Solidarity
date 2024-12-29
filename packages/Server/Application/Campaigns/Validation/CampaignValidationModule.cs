namespace Solidarity.Application.Campaigns.Validation;

public sealed class CampaignValidationModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/api/campaign/{id}/initiate-validation", async (int id, IDatabase database, IAuthenticatedAccount authenticatedAccount, IPaymentMethodProvider paymentMethodProvider) =>
		{
			var campaign = await database.Get<Campaign>(id);
			await campaign.TransitionToValidationPhase(database, authenticatedAccount, paymentMethodProvider);
		});

		endpoints.MapGet("/api/campaign/{id}/vote", async (IDatabase database, IAuthenticatedAccount authenticatedAccount, int id) =>
		{
			var campaign = await database.Get<Campaign>(id);
			return campaign.GetVoteByAccountId(authenticatedAccount.Id);
		});

		endpoints.MapGet("/api/campaign/{id}/votes", [AllowAnonymous] async (IDatabase database, IPaymentMethodProvider paymentMethodProvider, int id) =>
		{
			var campaign = await database.Get<Campaign>(id);
			return await campaign.GetVotes(database, paymentMethodProvider);
		});

		endpoints.MapPost("/api/campaign/{id}/vote", async (int id, [FromBody] bool vote, IDatabase database, IAuthenticatedAccount authenticatedAccount) =>
		{
			var campaign = await database.Get<Campaign>(id);
			await campaign.Vote(database, authenticatedAccount, vote);
		});
	}
}