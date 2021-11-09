namespace Solidarity.Domain.Models;

public class Handshake : Model
{
	public int AccountId { get; set; }

	public Account Account { get; set; } = null!;

	public string Phrase { get; set; } = null!;

	public DateTime Expiration { get; set; }
}