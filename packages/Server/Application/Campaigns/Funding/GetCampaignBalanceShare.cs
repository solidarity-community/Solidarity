namespace Solidarity.Application.Campaigns.Funding;

public sealed class GetCampaignBalanceShare(IDatabase database, IPaymentMethodProvider paymentMethodProvider)
{
	public async Task<double> Execute(int id, int? accountId = null)
	{
		var campaign = await database.Get<Campaign>(id);
		var account = accountId.HasValue is false ? null : await database.Get<Account>(accountId.Value);
		return await campaign.GetBalance(paymentMethodProvider, account);
	}
}