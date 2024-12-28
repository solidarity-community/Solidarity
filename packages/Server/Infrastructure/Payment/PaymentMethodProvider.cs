using System.Reflection;

namespace Solidarity.Infrastructure.Payment;

public sealed class PaymentMethodProvider : IPaymentMethodProvider, IHealthCheck
{
	private readonly IEnumerable<PaymentMethod> _enabledPaymentMethods;

	public PaymentMethodProvider(IServiceProvider serviceProvider)
	{
		var enabledPaymentMethods = EnvironmentVariables.PAYMENT_METHODS;
		_enabledPaymentMethods = typeof(PaymentMethod).Assembly
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(PaymentMethod)) && t.GetCustomAttribute<PaymentMethodAttribute>() is PaymentMethodAttribute paymentAttribute && enabledPaymentMethods?.Contains(paymentAttribute.Identifier) == true)
			.Select(t => (PaymentMethod)ActivatorUtilities.CreateInstance(serviceProvider, t));
	}

	public IEnumerable<PaymentMethod> GetAll() => _enabledPaymentMethods;

	public T Get<T>() where T : PaymentMethod
		=> _enabledPaymentMethods.FirstOrDefault(pm => pm is T) as T ?? throw new InvalidOperationException();

	public PaymentMethod Get(string identifier)
		=> _enabledPaymentMethods.First(pm => pm.Identifier == identifier) ?? throw new InvalidOperationException();

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		var healthCheckTasksByKey = _enabledPaymentMethods.ToDictionary(pm => pm.Name, pm => pm.CheckHealthAsync(context, cancellationToken));
		await Task.WhenAll(healthCheckTasksByKey.Values);
		var healthChecksByKey = healthCheckTasksByKey.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Result);

		return new HealthCheckResult(
			description: string.Join(", ", healthChecksByKey.Select(hc => hc.Value.Description)),
			status: healthChecksByKey switch
			{
				{ } when healthChecksByKey.Any(kvp => kvp.Value.Status == HealthStatus.Unhealthy) => HealthStatus.Unhealthy,
				{ } when healthChecksByKey.Any(kvp => kvp.Value.Status == HealthStatus.Degraded) => HealthStatus.Degraded,
				_ => HealthStatus.Healthy
			},
			data: healthChecksByKey!.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)
		);
	}
}