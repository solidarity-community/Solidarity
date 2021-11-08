using Solidarity.Application.Common;
using Solidarity.Application.Extensions;
using Solidarity.Application.Helpers;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Extensions;
using Solidarity.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Solidarity.Application.Services
{
	public class IdentityService : CrudService<Identity>
	{
		private readonly AccountService accountService;

		public IdentityService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService, AccountService accountService) : base(database, cryptoClientFactory, currentUserService)
		{
			this.accountService = accountService;
		}

		public Identity GetByAccountId(int? id)
		{
			id ??= currentUserService.Id;
			return database.Identities.FirstOrDefault(i => i.AccountId == id)
				?? throw new EntityNotFoundException<Identity>();
		}

		public Identity GetByUsername(string username)
		{
			var account = accountService.GetByUsername(username);
			return GetByAccountId(account.Id);
		}

		public override Identity Update(Identity identity)
		{
			if (Get(identity.Id).AccountId != identity.AccountId)
			{
				throw new AccountIdentityException("This identity does not belong to this account");
			}

			return base.Update(identity);
		}

		public Identity CreateOrUpdate(Identity identity)
		{
			return database.Identities.Any(i => i.AccountId == identity.AccountId)
				? Update(identity)
				: Create(identity);
		}
	}
}