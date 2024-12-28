namespace Solidarity.Application.Common;

public abstract class PeriodicBackgroundService(ILogger logger) : BackgroundService
{
	protected abstract TimeSpan Period { get; }

	protected abstract Task Execute(CancellationToken cancellationToken);

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				await Execute(cancellationToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "An error occurred executing the periodic background service.");
			}
			finally
			{
				await Task.Delay(Period, cancellationToken);
			}
		}
	}
}