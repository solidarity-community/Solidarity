namespace Solidarity.Application.Campaigns;

public class DeleteCampaign
{
	private readonly IPaymentMethodProvider _paymentMethodProvider;
	private readonly IDatabase _database;
	public DeleteCampaign(IPaymentMethodProvider paymentMethodProvider, IDatabase database) => (_paymentMethodProvider, _database) = (paymentMethodProvider, database);

	public async Task<Campaign> Execute(int id)
	{
		var campaign = await _database.Campaigns.Get(id);
		campaign.EnsureNotInStatus(CampaignStatus.Allocation);
		await campaign.Refund(_paymentMethodProvider);
		return await _database.Delete(campaign);
	}
}