namespace Solidarity.Application.Accounts.Profiles;

public class AccountProfileModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<GetAccountProfile>();
		services.AddTransient<CreateOrUpdateAccountProfile>();
		services.AddTransient<GetAccountByUsername>();
	}

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/account/profile", (GetAccountProfile getAccountProfile) => getAccountProfile.ExecuteAuthenticated());

		endpoints.MapGet("/account/profile/{id}", (GetAccountProfile getAccountProfile, int id) => getAccountProfile.Execute(id));

		endpoints.MapGet("/account/{accountId}/profile", (GetAccountProfile getAccountProfile, int accountId) => getAccountProfile.ExecuteByAccountId(accountId));

		var postAndPut = (CreateOrUpdateAccountProfile createOrUpdateAccountProfile, AccountProfile profile) => createOrUpdateAccountProfile.Execute(profile);
		endpoints.MapPost("/account/profile", postAndPut);
		endpoints.MapPut("/account/profile", postAndPut);
	}
}