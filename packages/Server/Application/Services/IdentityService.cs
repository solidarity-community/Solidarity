namespace Solidarity.Application.Services;

public class IdentityService : CrudService<Identity>
{
	private readonly AccountService _accountService;

	public IdentityService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, AccountService accountService) : base(database, paymentMethodProvider, currentUserService)
		=> _accountService = accountService;

	public async Task<Identity> GetByAccountId(int? id)
	{
		id ??= _currentUserService.Id;
		return await _database.Identities.FirstOrDefaultAsync(i => i.AccountId == id)
			?? throw new EntityNotFoundException<Identity>();
	}

	public async Task<Identity> GetByUsername(string username)
	{
		var account = _accountService.GetByUsername(username);
		return await GetByAccountId(account.Id);
	}

	public override async Task<Identity> Update(Identity identity)
	{
		return (await Get(identity.Id)).AccountId != identity.AccountId
			? throw new AccountIdentityException("This identity does not belong to this account")
			: await base.Update(identity);
	}

	public async Task<Identity> CreateOrUpdate(Identity identity)
	{
		return await _database.Identities.AnyAsync(i => i.AccountId == identity.AccountId)
			? await Update(identity)
			: await Create(identity);
	}
}