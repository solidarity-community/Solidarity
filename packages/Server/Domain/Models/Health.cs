namespace Solidarity.Domain.Models;

public class Health
{
	public HealthStatus Status { get; init; }
	public IEnumerable<HealthCheck> Checks { get; init; } = new List<HealthCheck>();
}

public class HealthCheck : Health
{
	public string Key { get; init; } = string.Empty;
}