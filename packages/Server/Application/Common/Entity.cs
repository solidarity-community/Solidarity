namespace Solidarity.Application.Common;

public abstract class Entity : Model
{
	public int Id { get; set; }
	public int? CreatorId { get; set; }
	public DateTime Creation { get; set; }
	public int? LastModifierId { get; set; }
	public DateTime LastModification { get; set; }
}