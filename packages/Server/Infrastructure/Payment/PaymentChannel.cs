namespace Solidarity.Infrastructure.Payment;

public abstract class PaymentChannel
{
	protected readonly PaymentMethod _paymentMethod;
	protected readonly IDatabase _database;
	protected readonly Campaign _campaign;
	public PaymentChannel(PaymentMethod paymentMethod, Campaign campaign)
	{
		_paymentMethod = paymentMethod;
		_database = paymentMethod._database;
		_campaign = campaign;
	}

	public abstract Task<string> GetDonationData(Account? account);
	public abstract Task<double> GetBalance(Account? account);
	public abstract Task<double> GetTotalBalance();

	protected readonly AllocationList _allocationList = new();

	public PaymentChannel RefundRemaining()
	{
		_allocationList.RefundRemaining = true;
		return this;
	}

	public PaymentChannel Refund(Account account)
	{
		_allocationList.AddRefund(account);
		return this;
	}

	public PaymentChannel RefundRange(IEnumerable<Account> accounts)
	{
		_allocationList.AddRefundRange(accounts);
		return this;
	}

	public PaymentChannel FundRemaining()
	{
		_allocationList.FundRemaining = true;
		return this;
	}

	public PaymentChannel Fund(Account account)
	{
		_allocationList.AddFund(account);
		return this;
	}

	public PaymentChannel FundRange(IEnumerable<Account> accounts)
	{
		_allocationList.AddFundRange(accounts);
		return this;
	}

	public abstract Task<IEnumerable<CampaignAllocationEntry>> Allocate();

	protected string AllocationDestination
		=> _campaign.ActivatedPaymentMethods.Find(pm => pm.Identifier == _paymentMethod.Identifier)?.AllocationDestination
			?? throw new InvalidOperationException($"No allocation destination found for the payment method {_paymentMethod.Name}");
}