namespace Solidarity.Infrastructure.Payment;

[AttributeUsage(AttributeTargets.Class)]
public class PaymentMethodAttribute : Attribute
{
	public string Identifier { get; set; }
	public PaymentMethodAttribute(string identifier) => Identifier = identifier;
}