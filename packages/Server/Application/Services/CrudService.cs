namespace Solidarity.Application.Services;

public abstract class CrudService<TModel> : Service where TModel : Model
{
	public CrudService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public virtual bool Exists(int id)
		=> _database.GetSet<TModel>().Find(id) != null;

	public virtual TModel Get(int id)
		=> _database.GetSet<TModel>().IncludeAll().First(x => x.Id == id) ?? throw new EntityNotFoundException<TModel>();

	public virtual IEnumerable<TModel> GetAll()
		=> _database.GetSet<TModel>().IncludeAll().AsNoTracking();

	public virtual TModel Create(TModel model)
	{
		_database.GetSet<TModel>().Add(model);
		_database.CommitChanges();
		return model;
	}

	public virtual TModel Update(TModel model)
	{
		_database.GetEntry(Get(model.Id)).CurrentValues.SetValues(model);
		_database.CommitChanges();
		return model;
	}

	public virtual TModel Delete(int id)
	{
		var model = Get(id);
		_database.GetSet<TModel>().Remove(model);
		_database.CommitChanges();
		return model;
	}
}