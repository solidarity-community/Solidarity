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

		endpoints.MapPut("/account",
			(AccountService accountService, Account account) => accountService.Update(account)
		);

		endpoints.MapGet("/account/{id}/reset",
			[AllowAnonymous] (AccountService accountService, int id) => accountService.Reset(id)
		);

		endpoints.MapGet("/account/recover",
			[AllowAnonymous] (AccountService accountService, string phrase) => accountService.Recover(phrase)
		);

		endpoints.MapGet("/identity/{id}", (IdentityService identityService, int id) => identityService.Get(id));

		endpoints.MapGet("/identity", (IdentityService identityService, [FromQuery] int? accountId) => identityService.GetByAccountId(accountId));

		var postAndPut = (IdentityService identityService, Identity identity) => identityService.CreateOrUpdate(identity);
		endpoints.MapPost("/identity", postAndPut);
		endpoints.MapPut("/identity", postAndPut);
	}
}