namespace Solidarity.Infrastructure.Payment;

public class PaymentMethodProvider : IPaymentMethodProvider, IHealthCheck
{
	private readonly IEnumerable<PaymentMethod> _enabledPaymentMethods;

	public PaymentMethodProvider(IServiceProvider serviceProvider)
	{
		var enabledPaymentMethods = Environment.GetEnvironmentVariable("PAYMENT_METHODS")?.Split(',').Select(x => x.Trim());
		_enabledPaymentMethods = typeof(PaymentMethod).Assembly
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(PaymentMethod)) && t.GetCustomAttribute<PaymentMethodAttribute>() is PaymentMethodAttribute paymentAttribute && enabledPaymentMethods?.Contains(paymentAttribute.Identifier) == true)
			.Select(t => (PaymentMethod)ActivatorUtilities.CreateInstance(serviceProvider, t));
	}

	public IEnumerable<PaymentMethod> GetAll() => _enabledPaymentMethods;

	public T Get<T>() where T : PaymentMethod
		=> _enabledPaymentMethods.FirstOrDefault(pm => pm is T) as T ?? throw new InvalidOperationException();

	public PaymentMethod Get(string Identifier)
		=> _enabledPaymentMethods.First(pm => pm.Identifier == Identifier) ?? throw new InvalidOperationException();

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		var healthChecks = await Task.WhenAll(_enabledPaymentMethods.Select(pm => pm.CheckHealthAsync(context, cancellationToken))) ?? Array.Empty<HealthCheckResult>();

		var unhealthy = healthChecks.Where(hc => hc.Status == HealthStatus.Unhealthy);
		var degraded = healthChecks.Where(hc => hc.Status == HealthStatus.Degraded);

		return unhealthy.Any()
			? HealthCheckResult.Unhealthy($"{unhealthy.Count()} unhealthy payment method(s): {string.Join(", ", unhealthy.Select(hc => hc.Description))}")
			: degraded.Any()
				? HealthCheckResult.Degraded($"{degraded.Count()} degraded payment method(s): {string.Join(", ", degraded.Select(hc => hc.Description))}")
				: HealthCheckResult.Healthy();
	}
}