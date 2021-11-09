namespace Solidarity.Domain.Models;

public class Validation : Model
{
	public Campaign Campaign { get; set; } = null!;

	public List<Vote> Votes { get; set; } = null!;

	public DateTime Expiration { get; set; }
}