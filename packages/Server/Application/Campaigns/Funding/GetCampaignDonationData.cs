namespace Solidarity.Application.Campaigns.Funding;

public class GetCampaignDonationData
{
	private readonly GetAuthenticatedAccount _getAccount;
	private readonly IPaymentMethodProvider _paymentMethodProvider;
	private readonly IDatabase _database;
	public GetCampaignDonationData(GetAuthenticatedAccount getAccount, IPaymentMethodProvider paymentMethodProvider, IDatabase database)
		=> (_getAccount, _paymentMethodProvider, _database) = (getAccount, paymentMethodProvider, database);

	public async Task<Dictionary<string, string>> Execute(int campaignId)
	{
		var campaign = await _database.Campaigns.Get(campaignId);
		var activatedPaymentMethods = campaign.ActivatedPaymentMethods.Select(pm => _paymentMethodProvider.Get(pm.Identifier)).ToList();
		var account = await _getAccount.Execute();

		var totalDonations = await campaign.GetTotalBalance(_paymentMethodProvider);
		var publicDonations = await campaign.GetBalance(_paymentMethodProvider, null);
		var privateDonations = totalDonations - publicDonations;

		(account is null && privateDonations / campaign.TotalExpenditure <= CampaignValidation.ApprovalThresholdPercentage)
			.Throw(() => new PublicDonationsLockedException()).IfTrue();

		var data = await Task.WhenAll(activatedPaymentMethods.Select(pm => pm.GetChannel(campaign).GetDonationData(account)));
		return data
			.Select((d, i) => new { d, i = activatedPaymentMethods[i].Identifier })
			.ToDictionary(d => d.i, d => d.d);
	}
}
