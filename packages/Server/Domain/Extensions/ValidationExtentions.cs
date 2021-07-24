using Solidarity.Domain.Models;
using System.Collections.Generic;

namespace Solidarity.Domain.Extensions
{
	public static class ValidationExtentions
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
}