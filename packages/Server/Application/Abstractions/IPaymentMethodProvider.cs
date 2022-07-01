namespace Solidarity.Application.Abstractions;

[TransientService]
public interface IPaymentMethodProvider
{
	IEnumerable<PaymentMethod> GetAll();
	PaymentMethod Get(string Identifier);
	T Get<T>() where T : PaymentMethod;
}
