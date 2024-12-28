namespace Solidarity.Application.Common;

public static class DatabaseCrudExtensions
{
	public static async Task<bool> Exists<TModel>(this IDatabase database, int id, CancellationToken ct = default) where TModel : Entity
		=> await database.GetOrDefault<TModel>(id, ct) is not null;

	public static async Task<TModel?> GetOrDefault<TModel>(this IDatabase database, Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default) where TModel : Entity
		=> await database.GetSet<TModel>().FirstOrDefaultAsync(predicate, cancellationToken);

	public static Task<TModel?> GetOrDefault<TModel>(this IDatabase database, int id, CancellationToken ct = default) where TModel : Entity
		=> database.GetOrDefault<TModel>(x => x.Id == id, ct);

	public static async Task<TModel> Get<TModel>(this IDatabase database, Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default) where TModel : Entity
		=> await database.GetSet<TModel>().FirstOrDefaultAsync(predicate, cancellationToken) ?? throw new EntityNotFoundException<TModel>();

	public static async Task<TModel> Get<TModel>(this IDatabase database, int id, CancellationToken ct = default) where TModel : Entity
		=> await database.GetOrDefault<TModel>(id, ct) ?? throw new EntityNotFoundException<TModel>(id);

	public static async Task<TModel> GetAsNoTracking<TModel>(this IDatabase database, int id, CancellationToken ct = default) where TModel : Entity
		=> await GetAsNoTrackingOrDefault<TModel>(database, id, ct) ?? throw new EntityNotFoundException<TModel>(id);

	public static Task<TModel?> GetAsNoTrackingOrDefault<TModel>(this IDatabase database, int id, CancellationToken ct = default) where TModel : Entity
		=> database.GetSet<TModel>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

	public static async Task<IEnumerable<TModel>> GetAll<TModel>(this IDatabase database, CancellationToken ct = default) where TModel : Entity
		=> await database.GetSet<TModel>().AsNoTracking().ToArrayAsync(ct);

	public static async Task<TModel> Create<TModel>(this IDatabase database, TModel model, CancellationToken ct = default) where TModel : Model
	{
		database.GetSet<TModel>().Add(model);
		await database.CommitChanges(ct);
		return model;
	}

	public static async Task<TModel> Update<TModel>(this IDatabase database, TModel model, CancellationToken ct = default) where TModel : Model
	{
		database.GetSet<TModel>().Update(model);
		await database.CommitChanges(ct);
		return model;
	}

	public static async Task<TModel> Save<TModel>(this IDatabase database, TModel model, CancellationToken ct = default) where TModel : Entity
		=> model.IsNew ? await database.Create(model, ct) : await database.Update(model, ct);

	public static Task<int> Delete<TModel>(this IDatabase database, TModel model, CancellationToken ct = default) where TModel : Entity => database.Delete<TModel>(model.Id, ct);

	public static Task<int> Delete<TModel>(this IDatabase database, int id, CancellationToken ct = default) where TModel : Entity
		=> database.GetSet<TModel>().Where(x => x.Id == id).ExecuteDeleteAsync(ct);

	public static async Task RemoveOrphanedRelations<T>(this IDatabase database, System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken ct = default) where T : Entity
	{
		var set = database.GetSet<T>();
		var orphanedEntities = await set.Where(predicate).ToListAsync(ct);
		set.RemoveRange(orphanedEntities);
	}
}