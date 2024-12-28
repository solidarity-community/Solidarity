namespace Solidarity.Application.Auditing;

public sealed class AuditChange : Model
{
	public int Id { get; set; }
	public int? EntityKey { get; set; }
	public Audit Audit { get; set; } = null!;
	public DateTime Date => Audit.Date;
	public EntityState State { get; set; }
	public string? Message { get; set; }
}