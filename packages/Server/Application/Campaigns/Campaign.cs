namespace Solidarity.Application.Campaigns;

public enum CampaignStatus { Funding, Validation, Allocation }

public class Campaign : Model
{
	[Required, MaxLength(50)] public string Title { get; set; } = null!;

	public string Description { get; set; } = null!;

	public CampaignStatus Status => this switch
	{
		{ Validation: null, Allocation: null } => CampaignStatus.Funding,
		{ Validation: not null, Allocation: null } => CampaignStatus.Validation,
		{ Validation: not null, Allocation: not null } => CampaignStatus.Allocation,
		_ => throw new InvalidOperationException("Invalid campaign status")
	};

	public Geometry Location { get; set; } = null!;

	public List<CampaignMedia> Media { get; set; } = new();

	public List<CampaignExpenditure> Expenditures { get; set; } = new();

	public long TotalExpenditure => Expenditures.Sum(e => e.TotalPrice);

	public List<CampaignPaymentMethod> ActivatedPaymentMethods { get; set; } = new();

	public int? ValidationId { get; set; }
	public CampaignValidation? Validation { get; set; }

	public int? AllocationId { get; set; }
	public CampaignAllocation? Allocation { get; set; }

	public void TransitionToValidationPhase(double totalBalance)
	{
		EnsureNotInStatus(CampaignStatus.Allocation, CampaignStatus.Validation);

		if (totalBalance < TotalExpenditure)
		{
			throw new InvalidOperationException("The campaign does not have enough funds.");
		}

		Validation ??= new CampaignValidation { Campaign = this };
	}

	public void ValidateForCreation()
	{
		Validate();
		EnsureTotalExpenditureNotTooLow();
		ValidationId = null;
		Validation = null;
		AllocationId = null;
		Allocation = null;
	}

	public void Update(Campaign updated)
	{
		updated.Validate();
		updated.EnsureTotalExpenditureNotTooLow();

		if (Status is not CampaignStatus.Funding && Location != updated.Location)
		{
			throw new InvalidOperationException("Cannot change location once the campaign is in validation status");
		}

		if (Status is not CampaignStatus.Funding && Enumerable.SequenceEqual(
			Expenditures.Select(e => e.TotalPrice),
			updated.Expenditures.Select(e => e.TotalPrice)) == false)
		{
			throw new InvalidOperationException("Cannot change expenditures once the campaign is in validation status");
		}

		if (Status is CampaignStatus.Allocation && Enumerable.SequenceEqual(
			ActivatedPaymentMethods.Select(pm => (pm.Identifier, pm.AllocationDestination)),
			updated.ActivatedPaymentMethods.Select(pm => (pm.Identifier, pm.AllocationDestination))) == false)
		{
			throw new InvalidOperationException("Cannot change activated payment methods once the campaign is in allocation status");
		}

		Media = updated.Media;
		Expenditures = updated.Expenditures;
		ActivatedPaymentMethods = updated.ActivatedPaymentMethods;
	}

	public async Task<double> GetBalance(IPaymentMethodProvider paymentMethodProvider, Account? account)
	{
		var balances = await Task.WhenAll(
			ActivatedPaymentMethods.Select(pm => paymentMethodProvider
				.Get(pm.Identifier)
				.GetChannel(this)
				.GetBalance(account)
			)
		);
		return balances?.Sum() ?? 0;
	}

	public void Vote(int accountId, bool value)
	{
		EnsureNotInStatus(CampaignStatus.Allocation, CampaignStatus.Funding);

		var vote = Validation!.Votes.Find(v => v.AccountId == accountId);
		if (vote is null)
		{
			vote = new() { ValidationId = Validation!.Id, AccountId = accountId };
			Validation!.Votes.Add(vote);
		}

		vote.Value = value;
	}

	public async Task Refund(IPaymentMethodProvider paymentMethodProvider)
	{
		EnsureNotInStatus(CampaignStatus.Allocation);
		await Task.WhenAll(
			ActivatedPaymentMethods.Select(paymentMethod => paymentMethodProvider
				.Get(paymentMethod.Identifier)
				.GetChannel(this)
				.RefundRemaining()
				.Allocate()
			)
		);
	}

	public async Task Allocate(IPaymentMethodProvider paymentMethodProvider, List<Account> accountsToRefund)
	{
		EnsureNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);
		Allocation ??= new();
		foreach (var paymentMethod in ActivatedPaymentMethods)
		{
			var allocationEntries = await paymentMethodProvider
				.Get(paymentMethod.Identifier)
				.GetChannel(this)
				.RefundRange(accountsToRefund)
				.FundRemaining()
				.Allocate();
			Allocation.Entries.AddRange(allocationEntries);
		}
	}

	public void EnsureTotalExpenditureNotTooLow()
	{
		if (TotalExpenditure == 0)
		{
			throw new CampaignExpenditureTooLowException();
		}
	}

	public void EnsureNotInStatus(params CampaignStatus[] statuses)
	{
		if (statuses.Contains(Status) == true)
		{
			throw new CampaignStatusException(statuses);
		}
	}
}