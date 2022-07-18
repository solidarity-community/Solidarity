namespace Solidarity.Infrastructure.Payment;

public class InvalidAllocationDestinationException : InvalidOperationException
{
	public InvalidAllocationDestinationException(PaymentMethod paymentMethod, string allocationDestination)
		: base($"Invalid allocation destination for payment method '{paymentMethod.Name}': '{allocationDestination}'.") { }
}