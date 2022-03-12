namespace Solidarity.Domain.Models;

public class Campaign : Model
{
	public Account? Creator { get; set; }

	[MaxLength(50), Required(ErrorMessage = "Title cannot be empty")]
	public string Title { get; set; } = string.Empty;

	[Required(ErrorMessage = "Description cannot be empty")]
	public string Description { get; set; } = null!;

	public Point Location { get; set; } = null!;

	public DateTime? Completion { get; set; }

	public List<DonationChannel> DonationChannels { get; set; } = new();

	public int? ValidationId { get; set; }

	public Validation? Validation { get; set; } = null!;

	// public List<Milestone> Milestones { get; set; }
}