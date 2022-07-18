namespace Solidarity.Application.Campaigns;

public class CampaignAllocationBackgroundService : BackgroundService
{
	private static readonly TimeSpan DelayTimeSpan = TimeSpan.FromSeconds(20);

	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger _logger;
	public CampaignAllocationBackgroundService(IServiceProvider serviceProvider, ILogger<CampaignAllocationBackgroundService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (stoppingToken.IsCancellationRequested is false)
		{
			using var scope = _serviceProvider.CreateScope();
			try
			{
				await scope.ServiceProvider.GetRequiredService<CampaignService>().AllocateAllValidated();
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, "Campaigns could not be processed.");
			}
			finally
			{
				await Task.Delay(DelayTimeSpan, stoppingToken);
			}
		}
	}
}