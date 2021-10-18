using NBitcoin;
using Solidarity.Application.Common;
using Solidarity.Core.Application;
using Solidarity.Domain.Enums;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solidarity.Application.Services
{
	public class CampaignService : Service
	{
		public CampaignService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService) { }

		public Campaign Get(int id)
		{
			return database.Campaigns.Find(id)?.WithoutAuthenticationData()
				?? throw new EntityNotFoundException("Campaign Not found");
		}

		public IEnumerable<Campaign> GetAll()
		{
			return database.Campaigns
				.IncludeAll()
				.WithoutAuthenticationData();
		}

		public decimal GetBalance(int id)
		{
			var campaign = Get(id);
			var client = cryptoClientFactory.GetClient(CoinType.Bitcoin, Domain.Enums.NetworkType.TestNet);
			var balance = campaign.DonationChannels
				.Select(dc => client.GetBalance(dc.WalletAddress))
				.Sum();
			return balance.ToDecimal(MoneyUnit.Satoshi);
		}

		public Campaign Create(Campaign campaign)
		{
			campaign.Completion = null;

			database.Campaigns.Add(campaign);
			database.CommitChanges();

			var cryptoClient = cryptoClientFactory.GetClient(CoinType.Bitcoin, Domain.Enums.NetworkType.TestNet);
			var address = cryptoClient.GetAddress(cryptoClient.DeriveKey(cryptoPrivateKey, campaign.Id));
			var addressText = address.ToString();
			if (addressText == null)
			{
				throw new Exception("Could not generate a crypto address for this campaign");
			}

			campaign.DonationChannels = new List<DonationChannel> {
				new DonationChannel {
					Campaign = campaign,
					WalletAddress = addressText,
					// This is the public wallet address
					Donor = null,
				}
			};

			database.CommitChanges();

			return campaign.WithoutAuthenticationData();
		}

		public Campaign Update(int id, Campaign model)
		{
			model.Id = id;
			var campaign = Get(id);

			if (campaign.CreatorId != currentUserService.Id)
			{
				throw new Exception("You are not allowed to edit this campaign");
			}

			campaign.Title = campaign.Title;
			campaign.Description = campaign.Description;

			database.CommitChanges();
			return campaign.WithoutAuthenticationData();
		}
	}
}