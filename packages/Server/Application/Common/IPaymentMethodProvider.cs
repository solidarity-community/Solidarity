namespace Solidarity.Application.Common;

public interface IPaymentMethodProvider
{
	IEnumerable<PaymentMethod> GetAll();
	PaymentMethod Get(string identifier);
	T Get<T>() where T : PaymentMethod;
}