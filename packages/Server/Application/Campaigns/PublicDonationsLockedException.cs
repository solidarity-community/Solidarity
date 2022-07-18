namespace Solidarity.Application.Campaigns;

[MapToHttpStatusCode(HttpStatusCode.NotAcceptable)]
public class PublicDonationsLockedException : Exception
{
	public PublicDonationsLockedException(string message = @"
		Public donations are locked for this campaign as more validators are required to approve allocating funds.
		Consider donating using an account and validating the campaign."
	) : base(message) { }
}