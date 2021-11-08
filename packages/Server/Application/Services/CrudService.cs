using NBitcoin;
using Solidarity.Application.Common;
using Solidarity.Core.Application;
using Solidarity.Domain.Enums;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solidarity.Application.Services
{
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
}