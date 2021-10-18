using Microsoft.EntityFrameworkCore;
using Solidarity.Application.Common;
using Solidarity.Domain.Models;

namespace Solidarity.Infrastructure.Persistance
{
	public class DatabaseContext : DbContext, IDatabase
	{
		private readonly ICurrentUserService currentUserService;

		public DatabaseContext(DbContextOptions<DatabaseContext> options, ICurrentUserService currentUserService) : base(options)
		{
			this.currentUserService = currentUserService;
			if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
			{
				Database.Migrate();
				DatabaseSeeder.Seed(this);
			}
		}

		public DbSet<Account> Accounts { get; set; } = null!;
		public DbSet<Domain.Models.Identity> Identities { get; set; } = null!;
		public DbSet<AuthenticationMethod> Authentications { get; set; } = null!;
		public DbSet<Handshake> Handshakes { get; set; } = null!;
		public DbSet<Campaign> Campaigns { get; set; } = null!;
		public DbSet<Validation> Validations { get; set; } = null!;
		public DbSet<Vote> Votes { get; set; } = null!;
		public DbSet<DonationChannel> DonationChannels { get; set; } = null!;
		public DbSet<CryptoMnemonic> CryptoMnemonics { get; set; } = null!;

		public void CommitChanges() => SaveChanges();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>(a => a.HasIndex(e => e.Username).IsUnique());
			modelBuilder.Entity<Account>().HasOne(a => a.Identity).WithOne(i => i!.Account);
			modelBuilder.Entity<Account>().HasMany(a => a.Campaigns).WithOne(c => c!.Creator);
			modelBuilder.Entity<Account>().HasMany(a => a.Votes).WithOne(v => v!.Account);

			modelBuilder.Entity<Handshake>().HasOne(h => h.Account);

			modelBuilder.Entity<AuthenticationMethod>().HasDiscriminator<string>("Type").HasValue<PasswordAuthentication>("password");

			modelBuilder.Entity<Campaign>().HasOne(c => c.Validation).WithOne(v => v.Campaign);
			modelBuilder.Entity<Campaign>().HasMany(c => c.DonationChannels).WithOne(dc => dc.Campaign);

			modelBuilder.Entity<Validation>().HasMany(v => v.Votes).WithOne(v => v.Validation);
		}

		public override int SaveChanges()
		{
			var userId = currentUserService.Id ?? 0;
			foreach (var entry in ChangeTracker.Entries<Model>())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatorId = userId;
						entry.Entity.Creation = System.DateTime.Now;
						break;
					case EntityState.Modified:
						entry.Entity.LastModifierId = userId;
						entry.Entity.LastModification = System.DateTime.Now;
						break;
				}
			}
			return base.SaveChanges();
		}
	}
}
