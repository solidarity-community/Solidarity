namespace Solidarity.Application.Services;

public abstract class CrudService<TModel> : Service where TModel : Model
{
	public CrudService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService) { }

	public virtual bool Exists(int id)
		=> database.GetSet<TModel>().Find(id) != null;

	public virtual TModel Get(int id)
		=> database.GetSet<TModel>().Find(id) ?? throw new EntityNotFoundException<TModel>();

	public virtual IEnumerable<TModel> GetAll()
		=> database.GetSet<TModel>().IncludeAll();

	public virtual TModel Create(TModel model)
	{
		database.GetSet<TModel>().Add(model);
		database.CommitChanges();
		return model;
	}

	public virtual TModel Update(TModel model)
	{
		database.GetEntry(Get(model.Id)).CurrentValues.SetValues(model);
		database.CommitChanges();
		return model;
	}
}