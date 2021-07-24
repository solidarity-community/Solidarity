using Solidarity.Domain.Models;
using System.Collections.Generic;

namespace Solidarity.Domain.Extensions
{
	public static class CampaignExtentions
	{
		public static Campaign WithoutAuthenticationData(this Campaign campaign)
		{
			campaign.Validation.WithoutAuthenticationData();
			campaign.Creator.WithoutAuthenticationData();
			return campaign;
		}

		public static IEnumerable<Campaign> WithoutAuthenticationData(this IEnumerable<Campaign> campaigns)
		{
			foreach (var campaign in campaigns)
			{
				campaign.WithoutAuthenticationData();
			}

			return campaigns;
		}
	}
}