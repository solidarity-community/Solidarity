namespace Solidarity.Application.Campaigns;

public class Validation : Model
{
	public Campaign Campaign { get; set; } = null!;

	public List<Vote> Votes { get; set; } = null!;

	public DateTime Expiration { get; set; }
}

public static class ValidationExtensions
{
	public static Validation WithoutAuthenticationData(this Validation validation)
	{
		validation.Campaign.WithoutAuthenticationData();
		return validation;
	}

	public static IEnumerable<Validation> WithoutAuthenticationData(this IEnumerable<Validation> validations)
	{
		foreach (var validation in validations)
		{
			validation.WithoutAuthenticationData();
		}

		return validations;
	}
}