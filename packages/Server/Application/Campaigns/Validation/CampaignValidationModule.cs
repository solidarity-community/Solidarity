namespace Solidarity.Application.Campaigns.Validation;

public sealed class CampaignValidationModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetVote>();
		services.AddTransient<GetVotes>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/api/campaign/{id}/initiate-validation",
			async (int id, IDatabase database, IAuthenticatedAccount authenticatedAccount, IPaymentMethodProvider paymentMethodProvider) =>
			{
				var campaign = await database.Get<Campaign>(id);
				await campaign.TransitionToValidationPhase(database, authenticatedAccount, paymentMethodProvider);
			}
		);

		endpoints.MapGet("/api/campaign/{id}/vote", (GetVote getVote, int id) => getVote.Execute(id));
		endpoints.MapGet("/api/campaign/{id}/votes", [AllowAnonymous] (GetVotes getVotes, int id) => getVotes.Execute(id));

		endpoints.MapPost("/api/campaign/{id}/vote", async (int id, [FromBody] bool vote, IDatabase database, IAuthenticatedAccount authenticatedAccount) =>
		{
			var campaign = await database.Get<Campaign>(id);
			await campaign.Vote(database, authenticatedAccount, vote);
		});
	}
}