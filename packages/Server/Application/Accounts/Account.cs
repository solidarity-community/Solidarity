namespace Solidarity.Application.Accounts;

public class Account : Entity
{
	[MaxLength(50), Required(ErrorMessage = "Username cannot be null")]
	private string username = null!;
	public string Username { get => username; set => username = value.ToLower(); }

	[Required(ErrorMessage = "Public-key cannot be null")]
	public string PublicKey { get; set; } = null!;

	public List<AuthenticationMethod> AuthenticationMethods { get; set; } = new();

	public List<Campaign> Campaigns { get; set; } = new();

	public List<CampaignValidationVote> Votes { get; set; } = new();

	public string IssueToken(TimeSpan expiration)
	{
		return TokenIssuer.IssueForAccount(this, expiration);
	}
}

public static class AccountExtensions
{
	public static Account WithoutCredentials(this Account account)
	{
		account.AuthenticationMethods = new();
		return account;
	}
}