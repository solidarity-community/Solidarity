namespace Solidarity.Infrastructure.Payment;

public sealed class InvalidPaymentMethodException(string identifier) : Exception($"A payment method with the identifier of \"${identifier}\" cannot be found.")
{
}