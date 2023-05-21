namespace Solidarity.Application.Campaigns;

public class SaveCampaign
{
	private readonly IDatabase _database;
	private readonly ICurrentUserService _currentUserService;
	private readonly IPaymentMethodProvider _paymentMethodProvider;
	public SaveCampaign(IDatabase database, ICurrentUserService currentUserService, IPaymentMethodProvider paymentMethodProvider)
		=> (_database, _currentUserService, _paymentMethodProvider) = (database, currentUserService, paymentMethodProvider);

	public async Task<Campaign> Create(Campaign campaign)
	{
		campaign.ValidateAllocationDestinations(_paymentMethodProvider);
		campaign.ValidateForCreation();
		await _database.Create(campaign);
		return campaign;
	}

	public async Task<Campaign> Update(Campaign campaign)
	{
		campaign.ValidateAllocationDestinations(_paymentMethodProvider);

		var entity = await _database.Campaigns.Get(campaign.Id);
		entity.EnsureCreatedByCurrentUser(_currentUserService);
		entity.Update(campaign);
		return await _database.Update(campaign);
	}
}
