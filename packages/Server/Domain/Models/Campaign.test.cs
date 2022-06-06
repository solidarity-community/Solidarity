namespace Solidarity.Domain.Models;

public class CampaignTest
{
	public static List<object[]> Data => new() {
		new object[] { Array.Empty<object>(), 0 },
		new object[] { new[] { (11, 1), (17, 2), (25, 0) }, 45 },
		new object[] { new[] { (11, 1), (17, 2), (25, 3) }, 120 },
	};

	[Theory(DisplayName = "TotalExpenditure should return the sum of campaign expenditures")]
	[MemberData(nameof(Data))]
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
}