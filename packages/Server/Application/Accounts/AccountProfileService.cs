namespace Solidarity.Application.Accounts;

[TransientService]
public class AccountProfileService : CrudService<AccountProfile>
{
	private readonly AccountService _accountService;

	public AccountProfileService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService, AccountService accountService) : base(database, paymentMethodProvider, currentUserService)
		=> _accountService = accountService;

	public async Task<AccountProfile> GetByAccountId(int? accountId)
	{
		accountId ??= _currentUserService.Id;
		return await _database.AccountProfiles.FirstOrDefaultAsync(i => i.AccountId == accountId)
			?? throw new EntityNotFoundException<AccountProfile>();
	}

	public async Task<AccountProfile> GetByUsername(string username)
	{
		var account = _accountService.GetByUsername(username);
		return await GetByAccountId(account.Id);
	}

	public override async Task<AccountProfile> Update(AccountProfile profile)
	{
		return (await Get(profile.Id)).AccountId != profile.AccountId
			? throw new AccountProfileException()
			: await base.Update(profile);
	}

	public async Task<AccountProfile> CreateOrUpdate(AccountProfile profile)
	{
		return await _database.AccountProfiles.AnyAsync(i => i.AccountId == profile.AccountId)
			? await Update(profile)
			: await Create(profile);
	}
}