namespace Solidarity.Domain.Models;

public class PaymentMethodKey : Model
{
	public string PaymentMethodIdentifier { get; set; } = null!;
	public string Key { get; set; } = null!;
}