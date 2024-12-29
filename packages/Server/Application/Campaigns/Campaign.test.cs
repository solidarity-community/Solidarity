namespace Solidarity.Application.Campaigns;

public sealed class CampaignTest
{
	[Fact(DisplayName = "TotalExpenditure should return the sum of campaign expenditures")]
	public void TestTotalExpenditure()
	{
		Campaign campaign = new()
		{
			Expenditures = [
				new() { UnitPrice = 11, Quantity = 1 },
				new() { UnitPrice = 17, Quantity = 2 },
				new() { UnitPrice = 25, Quantity = 3 }
			]
		};

		Assert.Equal(120, campaign.TotalExpenditure);
	}

	[Fact(DisplayName = "Status getter should return the correct status")]
	public void TestStatus()
	{
		Assert.Equal(CampaignStatus.Funding, new Campaign() { }.Status);
		Assert.Equal(CampaignStatus.Validation, new Campaign() { Validation = new() }.Status);
		Assert.Equal(CampaignStatus.Allocation, new Campaign() { Validation = new(), Allocation = new() }.Status);
	}
}