namespace Solidarity.Application.HealthChecks;

public class Health
{
	public Health() { }

	public Health(HealthReport healthReport)
	{
		Status = healthReport.Status;
		Checks = HealthCheck.FromDataDictionary(healthReport.Entries.ToDictionary(x => x.Key, x => (dynamic)x.Value));
	}

	public HealthStatus Status { get; init; }
	public IEnumerable<HealthCheck> Checks { get; init; } = new List<HealthCheck>();
}

public class HealthCheck : Health
{
	public static IEnumerable<HealthCheck> FromDataDictionary(IReadOnlyDictionary<string, dynamic> data)
	{
		data ??= new Dictionary<string, dynamic>();
		foreach (var (key, entry) in data)
		{
			yield return new HealthCheck
			{
				Key = key,
				Status = entry.Status,
				Checks = FromDataDictionary(entry.Data),
			};
		}
	}

	public string Key { get; init; } = string.Empty;
}