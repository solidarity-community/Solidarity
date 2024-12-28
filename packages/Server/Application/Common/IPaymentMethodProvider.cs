namespace Solidarity.Application.Common;

public interface IPaymentMethodProvider
{
	IEnumerable<PaymentMethod> GetAll();
	PaymentMethod Get(string Identifier);
	T Get<T>() where T : PaymentMethod;
}
