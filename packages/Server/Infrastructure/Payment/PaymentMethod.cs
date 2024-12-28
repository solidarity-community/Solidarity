namespace Solidarity.Infrastructure.Payment;

public abstract class PaymentMethod(IDatabase database) : IHealthCheck
{
	public string Identifier =>
		(GetType().GetCustomAttributes(typeof(PaymentMethodAttribute), true).First() as PaymentMethodAttribute)?.Identifier!;

	public virtual string Name => Identifier;
	public readonly IDatabase database = database;

	protected string? Key
	{
		get => database.PaymentMethodKeys.FirstOrDefault(key => key.PaymentMethodIdentifier == Identifier)?.Key;
		set
		{
			if (value is null)
			{
				database.PaymentMethodKeys.Remove(
					database.PaymentMethodKeys.Find(value) ?? throw new InvalidOperationException()
				);
			}
			else
			{
				database.PaymentMethodKeys.Add(new()
				{
					Key = value,
					PaymentMethodIdentifier = Identifier
				});
			}

			database.CommitChanges().Wait();
		}
	}

	public abstract Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		=> CheckHealthAsync(cancellationToken);

	public abstract PaymentChannel GetChannel(Campaign campaign);

	public abstract bool IsAllocationDestinationValid(string allocationDestination);
	public void ValidateAllocationDestination(string allocationDestination)
	{
		if (!IsAllocationDestinationValid(allocationDestination))
		{
			throw new InvalidAllocationDestinationException(this, allocationDestination);
		}
	}
}