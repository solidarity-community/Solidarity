
namespace Solidarity.Application.Accounts;

public sealed class Account : Entity
{
	public static void ConfigureDatabase(ModelBuilder b)
	{
		b.Entity<Account>(a => a.HasIndex(e => e.Username).IsUnique());
		b.Entity<AccountRecoveryHandshake>().HasOne(h => h.Account);
	}

	[MaxLength(50), Required(ErrorMessage = "Username cannot be null")]
	public string Username { get; set => field = value.ToLower(); } = null!;

	[Required(ErrorMessage = "Public-key cannot be null")] public string PublicKey { get; set; } = null!;

	[MaxLength(150)] public string? Name { get; set; }
	public DateTime? BirthDate { get; set; }

	public string? Password { get; set; }

	public string IssueToken(TimeSpan expiration)
	{
		return TokenIssuer.IssueForAccount(this, expiration);
	}

	public Account WithoutCredentials()
	{
		var account = this.Adapt<Account>();
		account.Password = null;
		return account;
	}
}