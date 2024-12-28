namespace Solidarity.Infrastructure.Identity;

public sealed class AuthenticatedAccount(IHttpContextAccessor httpContextAccessor) : IAuthenticatedAccount
{
	public static readonly AsyncLocal<int?> systemAccountId = new();
	private static readonly AsyncLocal<bool> systemUser = new();

	public int? Id => systemUser.Value
		? systemAccountId.Value
		: httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value is not string value
			? null
			: int.Parse(value);

	public bool IsAuthenticated => Id is not null;

	public DisposableAction System => new(
		() => systemUser.Value = true,
		() => systemUser.Value = false
	);
}