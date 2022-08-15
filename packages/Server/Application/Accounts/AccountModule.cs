namespace Solidarity.Application.Accounts;

public class AccountModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/account",
			(AccountService accountService) => accountService.GetWithoutAuthentication()
		);

		endpoints.MapPost("/account",
			[AllowAnonymous] (AccountService accountService, Account account) => accountService.CreateAndIssueToken(account)
		);

		endpoints.MapGet("/account/is-username-available/{username}",
			[AllowAnonymous] (AccountService accountService, string username) => accountService.IsUsernameAvailable(username)
		);

		endpoints.MapPut("/account",
			(AccountService accountService, Account account) => accountService.Update(account)
		);

		endpoints.MapGet("/account/{username}/reset",
			[AllowAnonymous] (AccountService accountService, string username) => accountService.ResetByUsername(username)
		);

		endpoints.MapGet("/account/recover",
			[AllowAnonymous] (AccountService accountService, string phrase) => accountService.Recover(phrase)
		);

		endpoints.MapGet("/account/profile", (AccountProfileService accountProfileService) => accountProfileService.GetByAccountId(null));
		endpoints.MapGet("/account/{accountId}/profile", (AccountProfileService accountProfileService, int accountId) => accountProfileService.GetByAccountId(accountId));

		endpoints.MapGet("/account/profile/{id}", (AccountProfileService accountProfileService, int id) => accountProfileService.Get(id));

		var postAndPut = (AccountProfileService accountProfileService, AccountProfile profile) => accountProfileService.CreateOrUpdate(profile);
		endpoints.MapPost("/account/profile", postAndPut);
		endpoints.MapPut("/account/profile", postAndPut);
	}
}