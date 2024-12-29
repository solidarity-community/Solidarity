namespace Solidarity.Application.Campaigns.Funding;

public sealed class GetCampaignDonationData(GetAuthenticatedAccount getAccount, IPaymentMethodProvider paymentMethodProvider, IDatabase database)
{
	public async Task<Dictionary<string, string>> Execute(int campaignId)
	{
		var campaign = await database.Get<Campaign>(campaignId);
		var activatedPaymentMethods = campaign.ActivatedPaymentMethods.Select(pm => paymentMethodProvider.Get(pm.Identifier)).ToList();
		var account = await getAccount.Execute();

		var totalDonations = await campaign.GetTotalBalance(paymentMethodProvider);
		var publicDonations = await campaign.GetBalance(paymentMethodProvider, null);
		var privateDonations = totalDonations - publicDonations;

		(account is null && privateDonations / campaign.TotalExpenditure <= CampaignValidation.ApprovalThresholdPercentage)
			.Throw(() => new PublicDonationsLockedException()).IfTrue();

		var data = await Task.WhenAll(activatedPaymentMethods.Select(pm => pm.GetChannel(campaign).GetDonationData(account)));
		return data
			.Select((d, i) => new { d, i = activatedPaymentMethods[i].Identifier })
			.ToDictionary(d => d.i, d => d.d);
	}
}