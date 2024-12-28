namespace Solidarity.Infrastructure.Payment;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PaymentMethodAttribute(string identifier) : Attribute
{
	public string Identifier { get; set; } = identifier;
}