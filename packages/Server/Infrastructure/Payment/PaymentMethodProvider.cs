namespace Solidarity.Infrastructure.Payment;

public class PaymentMethodProvider : IPaymentMethodProvider
{
	private readonly IEnumerable<PaymentMethod> _paymentMethods;
	public PaymentMethodProvider(IServiceProvider serviceProvider)
	{
		_paymentMethods = typeof(PaymentMethod).Assembly
			.GetTypes()
			.Where(t =>
				t.IsSubclassOf(typeof(PaymentMethod))
				&& t.GetCustomAttribute<PaymentMethodAttribute>() != null
			).Select(t => (PaymentMethod)ActivatorUtilities.CreateInstance(serviceProvider, t));
	}

	public PaymentMethod Get(string Identifier)
	{
		return _paymentMethods.First(pm => pm.Identifier == Identifier)
			?? throw new InvalidOperationException();
	}

	public IEnumerable<PaymentMethod> GetAll()
	{
		return _paymentMethods;
	}
}