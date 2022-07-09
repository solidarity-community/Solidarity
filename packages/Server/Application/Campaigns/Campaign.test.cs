namespace Solidarity.Application.Campaigns;

public class CampaignTest
{
	public static List<object[]> ExpenditureData => new() {
		new object[] { Array.Empty<object>(), 0 },
		new object[] { new[] { (11, 1), (17, 2), (25, 0) }, 45 },
		new object[] { new[] { (11, 1), (17, 2), (25, 3) }, 120 },
	};

	[Theory(DisplayName = "TotalExpenditure should return the sum of campaign expenditures")]
	[MemberData(nameof(ExpenditureData))]
	public void TestTotalExpenditure((int unitPrice, int quantity)[] expendituresList, int expected)
	{
		Campaign campaign = new()
		{
			Expenditures = expendituresList.Select(e => new CampaignExpenditure
			{
				UnitPrice = e.unitPrice,
				Quantity = e.quantity,
			}).ToList(),
		};

		Assert.Equal(expected, campaign.TotalExpenditure);
	}

	public static List<object[]> StatusData => new() {
		new object[] { null!, null!, CampaignStatus.Funding },
		new object[] { new CampaignValidation(), null!, CampaignStatus.Validation },
		new object[] {new CampaignValidation(), new CampaignAllocation(), CampaignStatus.Allocation },
	};

	[Theory(DisplayName = "Status getter should return the correct status")]
	[MemberData(nameof(StatusData))]
	public void TestStatus(CampaignValidation? validation, CampaignAllocation? allocation, CampaignStatus expected)
	{
		Campaign campaign = new()
		{
			Validation = validation,
			Allocation = allocation,
		};

		Assert.Equal(expected, campaign.Status);
	}
}