namespace Solidarity.Application.Campaigns;

public class CampaignExpenditureTooLowException : Exception
{
	public CampaignExpenditureTooLowException(string message = "Campaign expenditure is too low.") : base(message) { }
}