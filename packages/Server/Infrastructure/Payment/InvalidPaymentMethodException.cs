namespace Solidarity.Infrastructure.Payment;

public class InvalidPaymentMethodException : Exception
{
	public InvalidPaymentMethodException(string identifier)
		: base($"A payment method with the identifier of \"${identifier}\" cannot be found.") { }
}