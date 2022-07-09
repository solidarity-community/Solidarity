namespace Solidarity.Application.Campaigns;

public record CampaignVotes(decimal EndorsedBalance, decimal Balance, double ApprovalThreshold = 0.75);