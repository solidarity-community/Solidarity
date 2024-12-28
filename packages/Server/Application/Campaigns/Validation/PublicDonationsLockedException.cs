namespace Solidarity.Application.Campaigns.Validation;

[MapToHttpStatusCode(HttpStatusCode.NotAcceptable)]
public sealed class PublicDonationsLockedException(
	string message = """
	Public donations are locked for this campaign as more validators are required to approve allocating funds.
	Consider donating using an account and validating the campaign.
	"""
) : Exception(message)
{ }