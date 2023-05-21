namespace Solidarity.Application.Campaigns.Funding;

public class GetCampaignBalanceShare
{
	private readonly IDatabase _database;
	private readonly IPaymentMethodProvider _paymentMethodProvider;
	public GetCampaignBalanceShare(IDatabase database, IPaymentMethodProvider paymentMethodProvider) => (_database, _paymentMethodProvider) = (database, paymentMethodProvider);

	public async Task<double> Execute(int id, int? accountId = null)
	{
		var campaign = await _database.Campaigns.Get(id);
		var account = accountId.HasValue is false ? null : await _database.Accounts.Get(accountId.Value);
		return await campaign.GetBalance(_paymentMethodProvider, account);
	}
}