namespace Solidarity.Application.PaymentMethods;

public sealed class PaymentMethodKey : Entity
{
	[Key] public string PaymentMethodIdentifier { get; set; } = null!;
	public string Key { get; set; } = null!;
}