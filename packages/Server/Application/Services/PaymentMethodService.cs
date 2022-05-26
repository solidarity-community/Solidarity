namespace Solidarity.Application.Services;

public class PaymentMethodService : Service
{
	public PaymentMethodService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public IEnumerable<string> GetAllIdentifiers() => _paymentMethodProvider.GetAll().Select(pm => pm.Identifier);
}