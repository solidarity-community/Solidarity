namespace Solidarity.Application.Accounts;

public class AccountRecoveryHandshake : Model
{
	private static readonly TimeSpan DefaultExpirationTimeSpan = TimeSpan.FromMinutes(10);

	public int AccountId { get; set; }

	public Account Account { get; set; } = null!;

	public string Phrase { get; set; } = null!;

	public DateTime Expiration { get; set; } = DateTime.Now + DefaultExpirationTimeSpan;
}