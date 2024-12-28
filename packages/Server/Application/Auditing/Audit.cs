namespace Solidarity.Application.Auditing;

[NoAudit]
public sealed class Audit : Model
{
	public int Id { get; set; }
	public int? AccountId { get; set; }
	[AutoInclude] public Account? Account { get; set; }
	public DateTime Date { get; set; }
	[AutoInclude] public ICollection<AuditChange> Changes { get; } = [];
}