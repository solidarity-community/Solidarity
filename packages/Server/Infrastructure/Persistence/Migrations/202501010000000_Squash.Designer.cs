﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("202501010000000_Squash")]
partial class Squash
{
	/// <inheritdoc />
	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder
			.HasAnnotation("ProductVersion", "9.0.0")
			.HasAnnotation("Relational:MaxIdentifierLength", 128);

		SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

		modelBuilder.Entity("Solidarity.Application.Accounts.Account", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<DateTime?>("BirthDate").HasColumnType("datetime2");
			b.Property<string>("Name").HasMaxLength(150).HasColumnType("nvarchar(150)");
			b.Property<string>("Password").HasColumnType("nvarchar(max)");
			b.Property<string>("PublicKey").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Username").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
			b.HasKey("Id");
			b.HasIndex("Username").IsUnique();
			b.ToTable("Accounts");
		});

		modelBuilder.Entity("Solidarity.Application.Accounts.AccountRecoveryHandshake", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<int>("AccountId").HasColumnType("int");
			b.Property<DateTime>("Expiration").HasColumnType("datetime2");
			b.Property<string>("Phrase").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("AccountId");
			b.ToTable("AccountRecoveryHandshakes");
		});

		modelBuilder.Entity("Solidarity.Application.Auditing.Audit", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<int?>("AccountId").HasColumnType("int");
			b.Property<DateTime>("Date").HasColumnType("datetime2");
			b.HasKey("Id");
			b.HasIndex("AccountId");
			b.ToTable("Audits");
		});

		modelBuilder.Entity("Solidarity.Application.Auditing.AuditChange", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<int>("AuditId").HasColumnType("int");
			b.Property<int?>("EntityKey").HasColumnType("int");
			b.Property<string>("Message").HasColumnType("nvarchar(max)");
			b.Property<int>("State").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("AuditId");
			b.ToTable("AuditChange");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Allocation.CampaignAllocation", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.HasKey("Id");
			b.ToTable("CampaignAllocation");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Allocation.CampaignAllocationEntry", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<decimal>("Amount").HasColumnType("decimal(18,2)");
			b.Property<int>("CampaignAllocationId").HasColumnType("int");
			b.Property<string>("Data").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("PaymentMethodIdentifier").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Type").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("CampaignAllocationId");
			b.ToTable("CampaignAllocationEntry");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Campaign", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<int?>("AllocationId").HasColumnType("int");
			b.Property<int>("CreatorId").HasColumnType("int");
			b.Property<string>("Description").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<Geometry>("Location").IsRequired().HasColumnType("geography");
			b.Property<string>("Title").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
			b.Property<int?>("ValidationId").HasColumnType("int");
			b.HasKey("Id");
			b.HasIndex("AllocationId").IsUnique().HasFilter("[AllocationId] IS NOT NULL");
			b.HasIndex("CreatorId");
			b.HasIndex("ValidationId").IsUnique().HasFilter("[ValidationId] IS NOT NULL");
			b.ToTable("Campaigns");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignExpenditure", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<int>("CampaignId").HasColumnType("int");
			b.Property<string>("Name").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("Quantity").HasColumnType("int");
			b.Property<long>("UnitPrice").HasColumnType("bigint");
			b.HasKey("Id");
			b.HasIndex("CampaignId");
			b.ToTable("CampaignExpenditure");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignPaymentMethod", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<string>("AllocationDestination").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<int>("CampaignId").HasColumnType("int");
			b.Property<string>("Identifier").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("CampaignId");
			b.ToTable("CampaignPaymentMethod");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Media.CampaignMedia", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<int>("CampaignId").HasColumnType("int");
			b.Property<int>("Type").HasColumnType("int");
			b.Property<string>("Uri").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("CampaignId");
			b.ToTable("CampaignMedia");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Validation.CampaignValidation", b =>
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
			SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
			b.Property<DateTime>("Expiration").HasColumnType("datetime2");
			b.HasKey("Id");
			b.ToTable("CampaignValidation");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Validation.CampaignValidationVote", b =>
		{
			b.Property<int>("ValidationId").HasColumnType("int");
			b.Property<int>("AccountId").HasColumnType("int");
			b.Property<bool>("Value").HasColumnType("bit");
			b.HasKey("ValidationId", "AccountId");
			b.HasIndex("AccountId");
			b.ToTable("CampaignValidationVote");
		});

		modelBuilder.Entity("Solidarity.Application.PaymentMethods.PaymentMethodKey", b =>
		{
			b.Property<string>("PaymentMethodIdentifier").HasColumnType("nvarchar(450)");
			b.Property<int>("Id").HasColumnType("int");
			b.Property<string>("Key").IsRequired().HasColumnType("nvarchar(max)");
			b.HasKey("PaymentMethodIdentifier");
			b.ToTable("PaymentMethodKeys");
		});

		modelBuilder.Entity("Solidarity.Application.Accounts.AccountRecoveryHandshake", b =>
		{
			b.HasOne("Solidarity.Application.Accounts.Account", "Account").WithMany().HasForeignKey("AccountId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.Navigation("Account");
		});

		modelBuilder.Entity("Solidarity.Application.Auditing.Audit", b =>
		{
			b.HasOne("Solidarity.Application.Accounts.Account", "Account").WithMany().HasForeignKey("AccountId");
			b.Navigation("Account");
		});

		modelBuilder.Entity("Solidarity.Application.Auditing.AuditChange", b =>
		{
			b.HasOne("Solidarity.Application.Auditing.Audit", "Audit").WithMany("Changes").HasForeignKey("AuditId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.Navigation("Audit");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Allocation.CampaignAllocationEntry", b =>
		{
			b.HasOne("Solidarity.Application.Campaigns.Allocation.CampaignAllocation", "CampaignAllocation").WithMany("Entries").HasForeignKey("CampaignAllocationId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.Navigation("CampaignAllocation");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Campaign", b =>
		{
			b.HasOne("Solidarity.Application.Campaigns.Allocation.CampaignAllocation", "Allocation").WithOne("Campaign").HasForeignKey("Solidarity.Application.Campaigns.Campaign", "AllocationId");
			b.HasOne("Solidarity.Application.Accounts.Account", null).WithMany("Campaigns").HasForeignKey("CreatorId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.HasOne("Solidarity.Application.Campaigns.Validation.CampaignValidation", "Validation").WithOne("Campaign").HasForeignKey("Solidarity.Application.Campaigns.Campaign", "ValidationId");
			b.Navigation("Allocation");
			b.Navigation("Validation");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignExpenditure", b =>
		{
			b.HasOne("Solidarity.Application.Campaigns.Campaign", null).WithMany("Expenditures").HasForeignKey("CampaignId").OnDelete(DeleteBehavior.Cascade).IsRequired();
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignPaymentMethod", b =>
		{
			b.HasOne("Solidarity.Application.Campaigns.Campaign", "Campaign").WithMany("ActivatedPaymentMethods").HasForeignKey("CampaignId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.Navigation("Campaign");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Media.CampaignMedia", b =>
		{
			b.HasOne("Solidarity.Application.Campaigns.Campaign", null).WithMany("Media").HasForeignKey("CampaignId").OnDelete(DeleteBehavior.Cascade).IsRequired();
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Validation.CampaignValidationVote", b =>
		{
			b.HasOne("Solidarity.Application.Accounts.Account", "Account").WithMany("Votes").HasForeignKey("AccountId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.HasOne("Solidarity.Application.Campaigns.Validation.CampaignValidation", "Validation").WithMany("Votes").HasForeignKey("ValidationId").OnDelete(DeleteBehavior.Cascade).IsRequired();
			b.Navigation("Account");
			b.Navigation("Validation");
		});

		modelBuilder.Entity("Solidarity.Application.Accounts.Account", b =>
		{
			b.Navigation("Campaigns");
			b.Navigation("Votes");
		});

		modelBuilder.Entity("Solidarity.Application.Auditing.Audit", b =>
		{
			b.Navigation("Changes");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Allocation.CampaignAllocation", b =>
		{
			b.Navigation("Campaign").IsRequired();
			b.Navigation("Entries");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Campaign", b =>
		{
			b.Navigation("ActivatedPaymentMethods");
			b.Navigation("Expenditures");
			b.Navigation("Media");
		});

		modelBuilder.Entity("Solidarity.Application.Campaigns.Validation.CampaignValidation", b =>
		{
			b.Navigation("Campaign").IsRequired();
			b.Navigation("Votes");
		});
	}
}