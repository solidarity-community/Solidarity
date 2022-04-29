namespace Solidarity.Domain.Exceptions;

public class CampaignExpenditureTooLowException : Exception
{
	public CampaignExpenditureTooLowException(string message = "Campaign expenditure is too low.") : base(message) { }
}