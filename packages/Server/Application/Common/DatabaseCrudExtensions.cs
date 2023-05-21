namespace Solidarity.Application.Common;

public static class DatabaseCrudExtensions
{
	public static Task<bool> Any<TModel>(this IQueryable<TModel> queryable, int id) where TModel : Entity
		=> queryable.AnyAsync(x => x.Id == id);

	public static Task<TModel?> GetOrDefault<TModel>(this IQueryable<TModel> queryable, Expression<Func<TModel, bool>> predicate) where TModel : Entity
		=> queryable.FirstOrDefaultAsync(predicate);

	public static Task<TModel?> GetOrDefault<TModel>(this IQueryable<TModel> queryable, int id) where TModel : Entity
		=> queryable.GetOrDefault(x => x.Id == id);

	public static async Task<TModel> Get<TModel>(this IQueryable<TModel> queryable, int id) where TModel : Entity
		=> await queryable.GetOrDefault(id) ?? throw new EntityNotFoundException<TModel>();

	public static Task<TModel> Get<TModel>(this IQueryable<TModel> queryable, Expression<Func<TModel, bool>> predicate) where TModel : Entity
		=> queryable.Get(predicate);

	public static async Task<TModel> GetIgnoringAutoIncludes<TModel>(this IQueryable<TModel> queryable, int id) where TModel : Entity
		=> await queryable.IgnoreAutoIncludes().Get(id);

	public static async Task<TModel> GetAsNoTracking<TModel>(this IQueryable<TModel> queryable, int id) where TModel : Entity
		=> await queryable.AsNoTracking().Get(id);

	public static async Task<IEnumerable<TModel>> GetAll<TModel>(this IQueryable<TModel> queryable) where TModel : Entity
		=> await queryable.AsNoTracking().ToArrayAsync();

	public static async Task<TModel> Create<TModel>(this IDatabase database, TModel model) where TModel : Entity
	{
		database.GetSet<TModel>().Add(model);
		await database.CommitChangesAsync();
		return model;
	}

	public static async Task<TModel> Update<TModel>(this IDatabase database, TModel model) where TModel : Entity
	{
		database.GetSet<TModel>().Update(model);
		await database.CommitChangesAsync();
		return model;
	}

	public static Task<TModel> Delete<TModel>(this IDatabase database, TModel model) where TModel : Entity => database.Delete<TModel>(model.Id);

	public static async Task<TModel> Delete<TModel>(this IDatabase database, int id) where TModel : Entity
	{
		var model = await database.GetSet<TModel>().GetIgnoringAutoIncludes(id);
		database.GetSet<TModel>().Remove(model);
		await database.CommitChangesAsync();
		return model;
	}
}