namespace Solidarity.Application.Services;

public abstract class Service
{
	protected readonly IDatabase _database;
	protected readonly IPaymentMethodProvider _paymentMethodProvider;
	protected readonly ICurrentUserService _currentUserService;

	public Service(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService)
	{
		_database = database;
		_paymentMethodProvider = paymentMethodProvider;
		_currentUserService = currentUserService;
	}
}