namespace Solidarity.Application.Campaigns.Allocation;

public sealed class CampaignFundAllocator(IServiceProvider serviceProvider, ILogger<CampaignFundAllocator> logger) : PeriodicBackgroundService(logger)
{
	protected override TimeSpan Period => TimeSpan.FromSeconds(20);

	protected override async Task Execute(CancellationToken ct)
	{
		var database = serviceProvider.GetRequiredService<IDatabase>();
		var paymentMethodProvider = serviceProvider.GetRequiredService<IPaymentMethodProvider>();

		var campaigns = await database.Campaigns
			.Where(campaign => campaign.AllocationId == null && campaign.ValidationId != null && campaign.Validation!.Expiration < DateTime.Now)
			.ToArrayAsync(ct);

		foreach (var campaign in campaigns)
		{
			var votesStatus = await campaign.GetVotes(database, paymentMethodProvider);
			var endorsedRatio = votesStatus.EndorsedBalance / votesStatus.Balance;

			if (double.IsNaN(endorsedRatio))
			{
				await campaign.Delete(database, paymentMethodProvider);
				return;
			}

			await (
				endorsedRatio < votesStatus.ApprovalThreshold
					? campaign.Refund(paymentMethodProvider)
					: campaign.Fund(paymentMethodProvider)
			);

			await database.CommitChanges();
		}
	}
}