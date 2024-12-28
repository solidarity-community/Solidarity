namespace Solidarity.Infrastructure.Payment;

public sealed class AllocationList
{
	public bool FundRemaining
	{
		get;
		set
		{
			if (RefundRemaining == true)
			{
				throw new InvalidOperationException($"{nameof(RefundRemaining)} has already been called");
			}

			field = value;
		}
	}

	private readonly HashSet<Account> _funds = [];
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

	public bool RefundRemaining
	{
		get;
		set
		{
			if (FundRemaining == true)
			{
				throw new InvalidOperationException($"{nameof(FundRemaining)} has already been called");
			}

			if (Funds.Count > 0)
			{
				throw new InvalidOperationException($"Specific funding with rest refunds is not supported. Use specific refunds in conjunction with rest funds instead");
			}

			field = value;
		}
	}

	private readonly HashSet<Account> _refunds = [];
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