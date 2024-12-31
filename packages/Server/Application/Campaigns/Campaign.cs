

namespace Solidarity.Application.Campaigns;

public enum CampaignStatus { Funding, Validation, Allocation }

public sealed class Campaign : Entity, IValidatableObject
{
	public static void ConfigureDatabase(ModelBuilder b)
	{
		b.Entity<Campaign>().HasOne(c => c.Validation).WithOne(v => v.Campaign);
		b.Entity<Campaign>().HasOne(c => c.Allocation).WithOne(a => a.Campaign);
		b.Entity<Campaign>().HasMany(c => c.Media).WithOne();
		b.Entity<Campaign>().HasMany(c => c.Expenditures).WithOne();
		b.Entity<Campaign>().HasMany(c => c.ActivatedPaymentMethods).WithOne(pm => pm.Campaign);
		b.Entity<Campaign>().HasOne<Account>().WithMany().HasForeignKey(c => c.CreatorId);
	}

	public int CreatorId { get; set; }

	[Required, MaxLength(50)] public string Title { get; set; } = null!;
	[Required, MinLength(10)] public string Description { get; set; } = null!;

	public CampaignStatus Status => this switch
	{
		{ Validation: null, Allocation: null } => CampaignStatus.Funding,
		{ Validation: not null, Allocation: null } => CampaignStatus.Validation,
		{ Validation: not null, Allocation: not null } => CampaignStatus.Allocation,
		_ => throw new InvalidOperationException("Invalid campaign status")
	};

	[Required] public Geometry Location { get; set; } = null!;

	[AutoInclude] public List<CampaignMedia> Media { get; set; } = [];

	[AutoInclude] public List<CampaignExpenditure> Expenditures { get; set; } = [];

	public long TotalExpenditure => Expenditures.Sum(e => e.TotalPrice);

	[AutoInclude] public List<CampaignPaymentMethod> ActivatedPaymentMethods { get; set; } = [];

	public int? ValidationId { get; set; }
	[AutoInclude] public CampaignValidation? Validation { get; set; }

	public int? AllocationId { get; set; }
	[AutoInclude] public CampaignAllocation? Allocation { get; set; }

	public async Task TransitionToValidationPhase(IDatabase database, IAuthenticatedAccount authenticatedAccount, IPaymentMethodProvider paymentMethodProvider)
	{
		AssertNotInStatus(CampaignStatus.Allocation, CampaignStatus.Validation);
		(CreatorId != authenticatedAccount.Id).Throw("Operation is only allowed for the campaign creator.").IfTrue();

		var totalBalance = await GetTotalBalance(paymentMethodProvider);

		if (totalBalance < TotalExpenditure)
		{
			throw new InvalidOperationException("The campaign does not have enough funds.");
		}

		Validation ??= new CampaignValidation { Campaign = this };
		await database.CommitChanges();
	}

	IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
	{
		if (TotalExpenditure is 0 or < 0)
		{
			yield return new ValidationResult("Campaign expenditure is too low.", [nameof(TotalExpenditure)]);
		}
	}

	public async Task<Campaign> Save(IDatabase database, IAuthenticatedAccount authenticatedAccount, IPaymentMethodProvider paymentMethodProvider)
	{
		Validate();
		ActivatedPaymentMethods.ForEach(pm => paymentMethodProvider.Get(pm.Identifier).ValidateAllocationDestination(pm.AllocationDestination));

		var existing = await database.GetAsNoTrackingOrDefault<Campaign>(Id);
		CreatorId = existing?.CreatorId ?? authenticatedAccount.Id!.Value;

		if (existing is null)
		{
			ValidationId = null;
			Validation = null;
			AllocationId = null;
			Allocation = null;
		}
		else
		{
			if (existing.Status is not CampaignStatus.Funding && existing.Location != Location)
			{
				throw new InvalidOperationException("Cannot change location once the campaign is in validation status");
			}

			if (existing.Status is not CampaignStatus.Funding && Enumerable.SequenceEqual(
				existing.Expenditures.Select(e => e.TotalPrice),
				Expenditures.Select(e => e.TotalPrice)) == false)
			{
				throw new InvalidOperationException("Cannot change expenditures once the campaign is in validation status");
			}

			if (existing.Status is CampaignStatus.Allocation && Enumerable.SequenceEqual(
				existing.ActivatedPaymentMethods.Select(pm => (pm.Identifier, pm.AllocationDestination)),
				ActivatedPaymentMethods.Select(pm => (pm.Identifier, pm.AllocationDestination))) == false)
			{
				throw new InvalidOperationException("Cannot change activated payment methods once the campaign is in allocation status");
			}
		}

		return await database.Save(this);
	}

	public async Task Delete(IDatabase database, IPaymentMethodProvider paymentMethodProvider)
	{
		AssertNotInStatus(CampaignStatus.Allocation);
		await Refund(paymentMethodProvider);
		await database.Delete(this);
	}

	public async Task<double> GetBalance(IPaymentMethodProvider paymentMethodProvider, Account? account = null)
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

	public async Task<double> GetTotalBalance(IPaymentMethodProvider paymentMethodProvider)
	{
		var balances = await Task.WhenAll(
			ActivatedPaymentMethods.Select(pm => paymentMethodProvider
				.Get(pm.Identifier)
				.GetChannel(this)
				.GetTotalBalance()
			)
		);
		return balances?.Sum() ?? 0;
	}

	public async Task<double> GetBalanceShare(IDatabase database, IPaymentMethodProvider paymentMethodProvider, int? accountId = null)
	{
		var account = accountId.HasValue is false ? null : await database.Get<Account>(accountId.Value);
		return await GetBalance(paymentMethodProvider, account);
	}

	public async Task Vote(IDatabase database, IAuthenticatedAccount authenticatedAccount, bool value)
	{
		AssertNotInStatus(CampaignStatus.Allocation, CampaignStatus.Funding);
		Validation!.Votes.Cast(authenticatedAccount.Id!.Value, value);
		await database.CommitChanges();
	}

	public bool? GetVoteByAccountId(int? accountId) => Validation?.Votes.GetByAccountId(accountId);

	public record Votes(double EndorsedBalance, double Balance, double ApprovalThreshold);
	public async Task<Votes> GetVotes(IDatabase database, IPaymentMethodProvider paymentMethodProvider)
	{
		AssertNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);
		var balance = await GetTotalBalance(paymentMethodProvider);
		var endorsedDonations = 0d;
		foreach (var vote in Validation!.Votes.Where(vote => vote.Value == true))
		{
			endorsedDonations += await GetBalanceShare(database, paymentMethodProvider, vote.AccountId);
		}
		return new(endorsedDonations, balance, Validation!.ApprovalThreshold);

	}

	public async Task Refund(IPaymentMethodProvider paymentMethodProvider)
	{
		AssertNotInStatus(CampaignStatus.Allocation);

		var balance = await GetBalance(paymentMethodProvider);

		if (balance is 0)
		{
			return;
		}

		if (Status is CampaignStatus.Validation)
		{
			Allocation ??= new();
		}

		foreach (var paymentMethod in ActivatedPaymentMethods)
		{
			var allocationEntries = await paymentMethodProvider
				.Get(paymentMethod.Identifier)
				.GetChannel(this)
				.RefundRemaining()
				.Allocate();
			Allocation?.Entries.AddRange(allocationEntries);
		}
	}

	public async Task Fund(IPaymentMethodProvider paymentMethodProvider)
	{
		AssertNotInStatus(CampaignStatus.Funding, CampaignStatus.Allocation);
		Allocation ??= new();
		var accountsToRefund = Validation!.Votes
			.Where(vote => vote.Value is false)
			.Select(vote => vote.Account)
			.ToList();
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

	public void AssertNotInStatus(params CampaignStatus[] statuses)
		=> statuses.Throw(() => new CampaignStatusException(statuses)).IfContains(Status);
}