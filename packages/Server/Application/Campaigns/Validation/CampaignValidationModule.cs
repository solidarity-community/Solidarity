namespace Solidarity.Application.Campaigns.Validation;

public class CampaignValidationModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<InitiateCampaignValidation>();
		services.AddTransient<GetVote>();
		services.AddTransient<GetVotes>();
		services.AddTransient<VoteCampaign>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/campaign/{id}/initiate-validation",
			(InitiateCampaignValidation initiateCampaignValidation, int id) => initiateCampaignValidation.Execute(id)
		);

		endpoints.MapGet("/campaign/{id}/vote",
			(GetVote getVote, int id) => getVote.Execute(id)
		);

		endpoints.MapGet("/campaign/{id}/votes",
			[AllowAnonymous] (GetVotes getVotes, int id) => getVotes.Execute(id)
		);

		endpoints.MapPost("/campaign/{id}/vote",
			(VoteCampaign voteCampaign, int id, [FromBody] bool vote) => voteCampaign.Execute(id, vote)
		);
	}
}