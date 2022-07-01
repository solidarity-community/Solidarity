namespace Solidarity.Application.PaymentMethods;

public class PaymentMethodKey : Model
{
	public string PaymentMethodIdentifier { get; set; } = null!;
	public string Key { get; set; } = null!;
}