namespace Solidarity.Application.Abstractions;

public abstract class CrudService<TModel> : Service where TModel : Model
{
	public CrudService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public virtual async Task<bool> Exists(int id)
		=> await _database.GetSet<TModel>().FindAsync(id) != null;

	public virtual async Task<TModel> Get(int id)
		=> await _database.GetSet<TModel>().IncludeAll().FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException<TModel>();

	public virtual async Task<IEnumerable<TModel>> GetAll()
		=> await _database.GetSet<TModel>().IncludeAll().AsNoTracking().ToArrayAsync();

	public virtual async Task<TModel> Create(TModel model)
	{
		_database.GetSet<TModel>().Add(model);
		await _database.CommitChangesAsync();
		return model;
	}

	public virtual async Task<TModel> Update(TModel model)
	{
		var m = await Get(model.Id);
		_database.GetEntry(m).CurrentValues.SetValues(model);
		await _database.CommitChangesAsync();
		return model;
	}

	public virtual async Task<TModel> Delete(int id)
	{
		var model = await Get(id);
		_database.GetSet<TModel>().Remove(model);
		await _database.CommitChangesAsync();
		return model;
	}
}