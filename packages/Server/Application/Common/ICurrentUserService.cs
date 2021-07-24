namespace Solidarity.Application.Common
{
	public interface ICurrentUserService
	{
		int? Id { get; }
		bool IsAuthenticated { get; }
	}
}