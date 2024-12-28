namespace Solidarity.Application.Common;

public interface IAuthenticatedAccount
{
	int? Id { get; }
	bool IsAuthenticated { get; }
	public DisposableAction System { get; }
}

public sealed class DisposableAction : IDisposable
{
	private readonly Action dispose;
	public DisposableAction(Action action, Action dispose)
	{
		action();
		this.dispose = dispose;
	}

	public void Dispose()
	{
		dispose();
		GC.SuppressFinalize(this);
	}
}