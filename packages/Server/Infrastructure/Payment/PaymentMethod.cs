namespace Solidarity.Infrastructure.Payment;

public abstract class PaymentMethod
{
	protected readonly IDatabase _database;
	public PaymentMethod(IDatabase database) => _database = database;

	public string Identifier =>
		(GetType().GetCustomAttributes(typeof(PaymentMethodAttribute), true).First() as PaymentMethodAttribute)?.Identifier!;

	protected string? Key
	{
		get => _database.PaymentMethodKeys.FirstOrDefault(key => key.PaymentMethodIdentifier == Identifier)?.Key;
		set
		{
			if (value is null)
			{
				_database.PaymentMethodKeys.Remove(
					_database.PaymentMethodKeys.Find(value) ?? throw new InvalidOperationException()
				);
			}
			else
			{
				_database.PaymentMethodKeys.Add(new()
				{
					Key = value,
					PaymentMethodIdentifier = Identifier
				});
			}

			_database.CommitChanges();
		}
	}

	public abstract Task<decimal> GetBalance(Campaign campaign, Account? account);
	public abstract Task<string> GetDonationData(Campaign campaign, Account? account);
	public abstract Task Withdraw(Campaign campaign, string destination, decimal amount);
}