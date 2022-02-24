namespace Solidarity.Application.Services;

public class IdentityService : CrudService<Identity>
{
	private readonly AccountService _accountService;

	public IdentityService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, AccountService accountService) : base(database, paymentMethodProvider, currentUserService)
		=> _accountService = accountService;

	public Identity GetByAccountId(int? id)
	{
		id ??= _currentUserService.Id;
		return _database.Identities.FirstOrDefault(i => i.AccountId == id)
			?? throw new EntityNotFoundException<Identity>();
	}

	public Identity GetByUsername(string username)
	{
		var account = _accountService.GetByUsername(username);
		return GetByAccountId(account.Id);
	}

	public override Identity Update(Identity identity)
	{
		return Get(identity.Id).AccountId != identity.AccountId
			? throw new AccountIdentityException("This identity does not belong to this account")
			: base.Update(identity);
	}

	public Identity CreateOrUpdate(Identity identity)
	{
		return _database.Identities.Any(i => i.AccountId == identity.AccountId)
			? Update(identity)
			: Create(identity);
	}
}