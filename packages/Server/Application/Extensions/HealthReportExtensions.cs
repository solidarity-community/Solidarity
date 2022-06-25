namespace Solidarity.Application.Extensions;

public static class HealthReportExtensions
{
	public static Health ToHealth(this HealthReport healthReport)
	{
		return new Health
		{
			Status = healthReport.Status,
			Checks = ToHealthChecks(healthReport.Entries.ToDictionary(x => x.Key, x => (dynamic)x.Value)),
		};
	}

	private static IEnumerable<HealthCheck> ToHealthChecks(IReadOnlyDictionary<string, dynamic> data)
	{
		data ??= new Dictionary<string, dynamic>();
		foreach (var (key, entry) in data)
		{
			yield return new HealthCheck
			{
				Key = key,
				Status = entry.Status,
				Checks = ToHealthChecks(entry.Data),
			};
		}
	}
}