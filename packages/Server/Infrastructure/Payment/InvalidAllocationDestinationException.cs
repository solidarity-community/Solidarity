namespace Solidarity.Infrastructure.Payment;

public sealed class InvalidAllocationDestinationException(PaymentMethod paymentMethod, string allocationDestination)
	: InvalidOperationException($"Invalid allocation destination for payment method '{paymentMethod.Name}': '{allocationDestination}'.")
{
}