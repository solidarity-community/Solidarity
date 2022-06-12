namespace Solidarity.Infrastructure.Payment;

public class PaymentMethodProvider : IPaymentMethodProvider
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
}