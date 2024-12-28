namespace Solidarity.Infrastructure.Payment;

public abstract class PaymentChannel(PaymentMethod paymentMethod, Campaign campaign)
{
	public abstract Task<string> GetDonationData(Account? account);
	public abstract Task<double> GetBalance(Account? account);
	public abstract Task<double> GetTotalBalance();

	protected readonly AllocationList allocationList = new();

	public PaymentChannel RefundRemaining()
	{
		allocationList.RefundRemaining = true;
		return this;
	}

	public PaymentChannel Refund(Account account)
	{
		allocationList.AddRefund(account);
		return this;
	}

	public PaymentChannel RefundRange(IEnumerable<Account> accounts)
	{
		allocationList.AddRefundRange(accounts);
		return this;
	}

	public PaymentChannel FundRemaining()
	{
		allocationList.FundRemaining = true;
		return this;
	}

	public PaymentChannel Fund(Account account)
	{
		allocationList.AddFund(account);
		return this;
	}

	public PaymentChannel FundRange(IEnumerable<Account> accounts)
	{
		allocationList.AddFundRange(accounts);
		return this;
	}

	public abstract Task<IEnumerable<CampaignAllocationEntry>> Allocate();

	protected string AllocationDestination
		=> campaign.ActivatedPaymentMethods.Find(pm => pm.Identifier == paymentMethod.Identifier)?.AllocationDestination
			?? throw new InvalidOperationException($"No allocation destination found for the payment method {paymentMethod.Name}");
}