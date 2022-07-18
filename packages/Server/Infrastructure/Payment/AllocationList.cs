namespace Solidarity.Infrastructure.Payment;

public class AllocationList
{
	private bool fundRemaining = false;
	public bool FundRemaining
	{
		get => fundRemaining;
		set
		{
			if (RefundRemaining == true)
			{
				throw new InvalidOperationException($"{nameof(RefundRemaining)} has already been called");
			}

			fundRemaining = value;
		}
	}

	private readonly HashSet<Account> _funds = new();
	public ReadOnlyCollection<Account> Funds => _funds.ToList().AsReadOnly();

	public void AddFund(Account account)
	{
		if (_refunds.Contains(account))
		{
			throw new InvalidOperationException($"Account {account.Username} has already been refunded");
		}

		_funds.Add(account);
	}

	public void AddFundRange(IEnumerable<Account> accounts)
	{
		foreach (var account in accounts)
		{
			AddFund(account);
		}
	}

	private bool refundRemaining = false;
	public bool RefundRemaining
	{
		get => refundRemaining;
		set
		{
			if (FundRemaining == true)
			{
				throw new InvalidOperationException($"{nameof(FundRemaining)} has already been called");
			}

			if (Funds.Any())
			{
				throw new InvalidOperationException($"Specific funding with rest refunds is not supported. Use specific refunds in conjunction with rest funds instead");
			}

			refundRemaining = value;
		}
	}

	private readonly HashSet<Account> _refunds = new();
	public ReadOnlyCollection<Account> Refunds => _refunds.ToList().AsReadOnly();

	public void AddRefund(Account account)
	{
		if (_funds.Contains(account))
		{
			throw new InvalidOperationException($"Account {account.Username} already funds the campaign.");
		}

		_refunds.Add(account);
	}

	public void AddRefundRange(IEnumerable<Account> accounts)
	{
		foreach (var account in accounts)
		{
			AddRefund(account);
		}
	}

	public ReadOnlyCollection<Account> All => _funds.Concat(_refunds).ToList().AsReadOnly();
}