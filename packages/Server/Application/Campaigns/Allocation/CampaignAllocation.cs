
namespace Solidarity.Application.Campaigns.Allocation;

public sealed class CampaignAllocation : Entity
{
	public static void ConfigureDatabase(ModelBuilder b) => b.Entity<CampaignAllocation>().HasMany(a => a.Entries).WithOne(e => e.CampaignAllocation);

	public Campaign Campaign { get; set; } = null!;
	[AutoInclude] public List<CampaignAllocationEntry> Entries { get; set; } = [];
}