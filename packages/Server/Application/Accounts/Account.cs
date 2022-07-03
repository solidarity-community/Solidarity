namespace Solidarity.Application.Accounts;

public class Account : Model
{
	[MaxLength(50), Required(ErrorMessage = "Username cannot be null")]
	private string username = null!;
	public string Username { get => username; set => username = value.ToLower(); }

	[Required(ErrorMessage = "Public-key cannot be null")]
	public string PublicKey { get; set; } = null!;

	public List<AuthenticationMethod> AuthenticationMethods { get; set; } = new();

	public List<Campaign> Campaigns { get; set; } = new();

	public List<CampaignValidationVote> Votes { get; set; } = new();
}

public static class AccountExtensions
{
	public static Account WithoutAuthenticationData(this Account account)
	{
		account.AuthenticationMethods = new();
		return account;
	}
}