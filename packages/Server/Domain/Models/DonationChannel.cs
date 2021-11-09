namespace Solidarity.Domain.Models;

public class DonationChannel : Model
{
	public Campaign Campaign { get; set; } = null!;

	public string WalletAddress { get; set; } = null!;

	public Account? Donor { get; set; } = null!;
}