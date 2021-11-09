namespace Solidarity.Domain.Models;

public class Vote : Model
{
	public Validation Validation { get; set; } = null!;

	public Account Account { get; set; } = null!;

	public bool Value { get; set; }
}