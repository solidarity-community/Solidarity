namespace Solidarity.Application.Common;

public static class ValidationExtensions
{
	public static ICollection<ValidationResult>? GetValidationErrors(this object validatable)
	{
		var results = new List<ValidationResult>();
		Validator.TryValidateObject(validatable, new ValidationContext(validatable), results, true);
		return results.Count == 0 ? null : results;
	}

	public static void ValidateObject(this object validatable)
		=> Validator.ValidateObject(validatable, new ValidationContext(validatable), true);
}