namespace Solidarity.Application.Campaigns;

public enum CampaignStatus { Funding, Validation, Allocation }

public class Campaign : Model
{
	private const int ValidationTimeSpanInDays = 7;

	[Required, MaxLength(50)] public string Title { get; set; } = null!;

	public string Description { get; set; } = null!;

	public CampaignStatus Status => (Validation, Allocation) switch
	{
		(null, null) => CampaignStatus.Funding,
		(not null, null) => CampaignStatus.Validation,
		(not null, not null) => CampaignStatus.Allocation,
		_ => throw new Exception("Invalid campaign status")
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

	public void TransitionToValidationPhase(decimal totalBalance)
	{
		EnsureNotInStatus(CampaignStatus.Allocation, CampaignStatus.Validation);

		if (totalBalance < TotalExpenditure)
		{
			throw new InvalidOperationException("The campaign does not have enough funds.");
		}

		Validation ??= new CampaignValidation
		{
			Campaign = this,
			Expiration = DateTime.Now.AddDays(ValidationTimeSpanInDays),
		};
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

	public void ValidateForUpdate(Campaign updated)
	{
		updated.Validate();
		updated.EnsureTotalExpenditureNotTooLow();
		if (Status is not CampaignStatus.Funding && updated.TotalExpenditure != TotalExpenditure)
		{
			throw new InvalidOperationException("Cannot change expenditures once the campaign is in validation status");
		}
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

	public async Task Refund(Func<CampaignPaymentMethod, Task> refundPaymentMethod)
	{
		EnsureNotInStatus(CampaignStatus.Allocation);
		await Task.WhenAll(ActivatedPaymentMethods.Select(paymentMethod => refundPaymentMethod(paymentMethod)));
	}

	public async Task Allocate(Func<CampaignPaymentMethod, Task<string>> allocatePaymentMethod)
	{
		EnsureNotInStatus(CampaignStatus.Funding, CampaignStatus.Validation);
		Allocation ??= new();
		foreach (var paymentMethod in ActivatedPaymentMethods)
		{
			var data = await allocatePaymentMethod(paymentMethod);
			Allocation.Entries.Add(new()
			{
				CampaignAllocation = Allocation,
				PaymentMethodIdentifier = paymentMethod.Identifier,
				Data = data
			});
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