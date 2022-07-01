namespace Solidarity.Infrastructure.Identity;

[TransientService]
public class CurrentUserService : ICurrentUserService
{
	public int? Id { get; }
	public bool IsAuthenticated => Id is not null;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
		var value = claim?.Value;
		Id = value is null ? null : int.Parse(value);
	}
}