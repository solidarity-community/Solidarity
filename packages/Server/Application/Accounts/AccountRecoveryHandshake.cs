namespace Solidarity.Application.Accounts;

public sealed class AccountRecoveryHandshake : Entity
{
	private static readonly TimeSpan DefaultExpirationTimeSpan = TimeSpan.FromMinutes(10);

	public int AccountId { get; set; }
	[AutoInclude] public Account Account { get; set; } = null!;
	public string Phrase { get; set; } = null!;
	public DateTime Expiration { get; set; } = DateTime.UtcNow + DefaultExpirationTimeSpan;
}