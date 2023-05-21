namespace Solidarity.Application.Campaigns.Funding;

public class GetCampaignBalance
{
	private readonly IDatabase _database;
	private readonly IPaymentMethodProvider _paymentMethodProvider;
	public GetCampaignBalance(IDatabase database, IPaymentMethodProvider paymentMethodProvider) => (_database, _paymentMethodProvider) = (database, paymentMethodProvider);

	public async Task<double> Execute(int id)
	{
		var campaign = await _database.Campaigns.Get(id);
		return await campaign.GetTotalBalance(_paymentMethodProvider);
	}
}