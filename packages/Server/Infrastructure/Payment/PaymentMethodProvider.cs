namespace Solidarity.Infrastructure.Payment;

public class PaymentMethodProvider : IPaymentMethodProvider
{
	private readonly IEnumerable<PaymentMethod> _paymentMethods;

	public PaymentMethodProvider(IServiceProvider serviceProvider)
	{
		var enabledPaymentMethods = Environment.GetEnvironmentVariable("PAYMENT_METHODS")?.Split(',');
		_paymentMethods = typeof(PaymentMethod).Assembly
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(PaymentMethod)) && t.GetCustomAttribute<PaymentMethodAttribute>() != null)
			.Select(t => (PaymentMethod)ActivatorUtilities.CreateInstance(serviceProvider, t))
			.Where(p => enabledPaymentMethods?.Contains(p.Identifier) ?? false);
	}

	public IEnumerable<PaymentMethod> GetAll() => _paymentMethods;

	public PaymentMethod Get(string Identifier)
		=> _paymentMethods.First(pm => pm.Identifier == Identifier) ?? throw new InvalidOperationException();
}