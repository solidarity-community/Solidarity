namespace Solidarity.Application.PaymentMethods;

[TransientService]
public class PaymentMethodService : Service
{
	public PaymentMethodService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public IEnumerable<PaymentMethod> GetAll() => _paymentMethodProvider.GetAll();
	public PaymentMethod Get(string Identifier) => _paymentMethodProvider.Get(Identifier);
	public T Get<T>() where T : PaymentMethod => _paymentMethodProvider.Get<T>();
	public IEnumerable<string> GetAllIdentifiers() => GetAll().Select(pm => pm.Identifier);
}