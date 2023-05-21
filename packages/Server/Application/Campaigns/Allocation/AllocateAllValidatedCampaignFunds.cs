namespace Solidarity.Application.Campaigns.Allocation;

public class AllocateAllValidatedCampaignFunds
{
	private readonly IDatabase _database;
	private readonly IPaymentMethodProvider _paymentMethodProvider;
	private readonly GetVotes _getVotes;
	private readonly DeleteCampaign _delete;
	public AllocateAllValidatedCampaignFunds(IDatabase database, IPaymentMethodProvider paymentMethodProvider, GetVotes getVotes, DeleteCampaign delete)
		=> (_database, _paymentMethodProvider, _getVotes, _delete) = (database, paymentMethodProvider, getVotes, delete);

	public async Task Execute()
	{
		var campaigns = await _database.Campaigns
			.Where(campaign => campaign.AllocationId == null && campaign.ValidationId != null && campaign.Validation!.Expiration < DateTime.Now)
			.ToArrayAsync();

		foreach (var campaign in campaigns)
		{
			var votesStatus = await _getVotes.Execute(campaign.Id);

			var endorsedRatio = votesStatus.EndorsedBalance / votesStatus.Balance;

			if (double.IsNaN(endorsedRatio))
			{
				await _delete.Execute(campaign.Id);
				return;
			}

			await (endorsedRatio < votesStatus.ApprovalThreshold
				? campaign.Refund(_paymentMethodProvider)
				: campaign.Fund(_paymentMethodProvider));

			await _database.CommitChangesAsync();
		}
	}
}
