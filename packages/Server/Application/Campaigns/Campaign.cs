namespace Solidarity.Application.Campaigns;

public enum CampaignStatus { Funding, Allocation, Complete }

public class Campaign : Model
{
	private const int ValidationTimeSpanInDays = 3;

	[MaxLength(50)] public string Title { get; set; } = string.Empty;

	public string Description { get; set; } = null!;

	public CampaignStatus Status => (AllocationDate, CompletionDate) switch
	{
		(not null, null) => CampaignStatus.Allocation,
		(not null, not null) => CampaignStatus.Complete,
		_ => CampaignStatus.Funding
	};

	public Geometry Location { get; set; } = null!;

	public DateTime TargetAllocationDate { get; set; }

	public DateTime? AllocationDate { get; set; }

	public DateTime? CompletionDate { get; set; }

	public List<CampaignMedia> Media { get; set; } = new();

	public List<CampaignExpenditure> Expenditures { get; set; } = new();

	public long TotalExpenditure => Expenditures.Sum(e => e.TotalPrice);

	public List<CampaignPaymentMethod> ActivatedPaymentMethods { get; set; } = new();

	public int? ValidationId { get; set; }

	public CampaignValidation? Validation { get; set; }

	// public List<Milestone> Milestones { get; set; }

	public void TransitionToAllocationPhase()
	{
		TargetAllocationDate = DateTime.Now;
		AllocationDate = DateTime.Now;
		Validation ??= new CampaignValidation
		{
			Campaign = this,
			Expiration = DateTime.Now.AddDays(ValidationTimeSpanInDays),
		};
	}
}