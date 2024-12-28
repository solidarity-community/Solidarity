namespace Solidarity.Application.Campaigns.Funding;

public sealed class GetCampaignBalance(IDatabase database, IPaymentMethodProvider paymentMethodProvider)
{
	public async Task<double> Execute(int id)
	{
		var campaign = await database.Get<Campaign>(id);
		return await campaign.GetTotalBalance(paymentMethodProvider);
	}
}