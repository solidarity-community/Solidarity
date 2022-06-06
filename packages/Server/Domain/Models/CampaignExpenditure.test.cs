namespace Solidarity.Domain.Models;

public class CampaignExpenditureTest
{
	[Theory(DisplayName = "TotalPrice should return the multiplication of UnitPrice and Quantity expenditures")]
	[InlineData(1, 0, 0), InlineData(0, 1, 0)]
	[InlineData(12, 11, 132), InlineData(11, 12, 132)]
	public void TestTotalPrice(int quantity, int unitPrice, int expected)
	{
		CampaignExpenditure campaignExpenditure = new()
		{
			Quantity = quantity,
			UnitPrice = unitPrice,
		};

		Assert.Equal(expected, campaignExpenditure.TotalPrice);
	}
}