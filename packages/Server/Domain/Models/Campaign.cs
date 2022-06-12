namespace Solidarity.Domain.Models;

public class Campaign : Model
{
	[MaxLength(50)] public string Title { get; set; } = string.Empty;

	public string Description { get; set; } = null!;

	public Geometry Location { get; set; } = null!;

	public DateTime TargetDate { get; set; }

	public DateTime? Completion { get; set; }

	public List<CampaignMedia> Media { get; set; } = new();

	public List<CampaignExpenditure> Expenditures { get; set; } = new();

	public long TotalExpenditure => Expenditures.Sum(e => e.TotalPrice);

	public List<CampaignPaymentMethod> ActivatedPaymentMethods { get; set; } = new();

	public int? ValidationId { get; set; }

	public Validation? Validation { get; set; } = null!;

	// public List<Milestone> Milestones { get; set; }
}