using System.Reflection;

namespace Solidarity.Application.Auditing;

[AttributeUsage(AttributeTargets.Class)]
public sealed class NoAuditAttribute : Attribute { }

public sealed class AuditSaveChangesInterceptor(GetAuthenticatedAccount getAuthenticatedAccount) : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
	{
		var database = (IDatabase)eventData.Context!;

		var changesToAudit = database.ChangeTracker.Entries()
			.Where(entity => entity.Entity.GetType().GetCustomAttribute<NoAuditAttribute>(inherit: true) is null)
			.Where(entity => entity.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

		if (changesToAudit.Any())
		{
			Audit audit = new()
			{
				Date = DateTime.UtcNow,
				AccountId = (await getAuthenticatedAccount.Execute())?.Id,
			};

			static string BuildMessage(string operation, EntityEntry entry)
				=> entry.Properties.Aggregate($"{operation} {entry.Metadata.DisplayName()} with\n",
					(auditString, property) => auditString + $"\t{property.Metadata.Name}: '{property.CurrentValue}'\n");

			foreach (var entry in changesToAudit)
			{
				audit.Changes.Add(new AuditChange
				{
					EntityKey = entry.Metadata.FindPrimaryKey()?.Properties
						.Select(property => property.PropertyInfo?.GetValue(entry.Entity))
						.OfType<int>()
						.FirstOrDefault(),
					State = entry.State,
					Message = entry.State switch
					{
						EntityState.Added => BuildMessage("Inserting", entry),
						EntityState.Modified => BuildMessage("Updating", entry),
						EntityState.Deleted => BuildMessage("Deleting", entry),
						_ => null
					}
				});
			}

			database.Audits.Add(audit);
		}

		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}
}