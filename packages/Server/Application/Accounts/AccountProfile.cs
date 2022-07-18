namespace Solidarity.Application.Accounts;

public class AccountProfile : Model
{
	public int AccountId { get; set; }

	public Account? Account { get; set; } = null!;

	[MaxLength(50)]
	public string FirstName { get; set; } = null!;

	[MaxLength(50)]
	public string LastName { get; set; } = null!;

	public DateTime? BirthDate { get; set; }
}