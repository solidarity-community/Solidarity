﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Solidarity.Infrastructure.Persistence;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Solidarity.Application.Accounts.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<string>("PublicKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Solidarity.Application.Accounts.AccountProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("AccountProfiles");
                });

            modelBuilder.Entity("Solidarity.Application.Accounts.AccountRecoveryHandshake", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<string>("Phrase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountRecoveryHandshakes");
                });

            modelBuilder.Entity("Solidarity.Application.Authentication.AuthenticationMethod", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("Salt")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.HasKey("AccountId", "Type", "Salt");

                    b.ToTable("AuthenticationMethods");

                    b.HasDiscriminator<int>("Type");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AllocationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<Geometry>("Location")
                        .IsRequired()
                        .HasColumnType("geography");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("ValidationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AllocationId")
                        .IsUnique()
                        .HasFilter("[AllocationId] IS NOT NULL");

                    b.HasIndex("CreatorId");

                    b.HasIndex("ValidationId")
                        .IsUnique()
                        .HasFilter("[ValidationId] IS NOT NULL");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignAllocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CampaignAllocation");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignAllocationEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("CampaignAllocationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<string>("PaymentMethodIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CampaignAllocationId");

                    b.ToTable("CampaignAllocationEntry");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignExpenditure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CampaignId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<long>("UnitPrice")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("CampaignExpenditures");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignMedia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CampaignId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("CampaignMedia");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignPaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AllocationDestination")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CampaignId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("CampaignPaymentMethods");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignValidation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CampaignValidations");
                });

            modelBuilder.Entity("Solidarity.Application.PaymentMethods.PaymentMethodKey", b =>
                {
                    b.Property<string>("PaymentMethodIdentifier")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.HasKey("PaymentMethodIdentifier");

                    b.ToTable("PaymentMethodKeys");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.CampaignValidationVote", b =>
                {
                    b.Property<int>("ValidationId")
                        .HasColumnType("int");

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Creation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<bool>("Value")
                        .HasColumnType("bit");

                    b.HasKey("ValidationId", "AccountId");

                    b.HasIndex("AccountId");

                    b.ToTable("CampaignValidationVotes");
                });

            modelBuilder.Entity("Solidarity.Application.Authentication.PasswordAuthenticationMethod", b =>
                {
                    b.HasBaseType("Solidarity.Application.Authentication.AuthenticationMethod");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Solidarity.Application.Accounts.AccountProfile", b =>
                {
                    b.HasOne("Solidarity.Application.Accounts.Account", "Account")
                        .WithOne()
                        .HasForeignKey("Solidarity.Application.Accounts.AccountProfile", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Solidarity.Application.Accounts.AccountRecoveryHandshake", b =>
                {
                    b.HasOne("Solidarity.Application.Accounts.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Solidarity.Application.Authentication.AuthenticationMethod", b =>
                {
                    b.HasOne("Solidarity.Application.Accounts.Account", "Account")
                        .WithMany("AuthenticationMethods")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.Campaign", b =>
                {
                    b.HasOne("Solidarity.Application.Campaigns.CampaignAllocation", "Allocation")
                        .WithOne("Campaign")
                        .HasForeignKey("Solidarity.Application.Campaigns.Campaign", "AllocationId");

                    b.HasOne("Solidarity.Application.Accounts.Account", null)
                        .WithMany("Campaigns")
                        .HasForeignKey("CreatorId");

                    b.HasOne("Solidarity.Application.Campaigns.CampaignValidation", "Validation")
                        .WithOne("Campaign")
                        .HasForeignKey("Solidarity.Application.Campaigns.Campaign", "ValidationId");

                    b.Navigation("Allocation");

                    b.Navigation("Validation");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignAllocationEntry", b =>
                {
                    b.HasOne("Solidarity.Application.Campaigns.CampaignAllocation", "CampaignAllocation")
                        .WithMany("Entries")
                        .HasForeignKey("CampaignAllocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CampaignAllocation");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignExpenditure", b =>
                {
                    b.HasOne("Solidarity.Application.Campaigns.Campaign", null)
                        .WithMany("Expenditures")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignMedia", b =>
                {
                    b.HasOne("Solidarity.Application.Campaigns.Campaign", null)
                        .WithMany("Media")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignPaymentMethod", b =>
                {
                    b.HasOne("Solidarity.Application.Campaigns.Campaign", "Campaign")
                        .WithMany("ActivatedPaymentMethods")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.CampaignValidationVote", b =>
                {
                    b.HasOne("Solidarity.Application.Accounts.Account", "Account")
                        .WithMany("Votes")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Solidarity.Application.Campaigns.CampaignValidation", "Validation")
                        .WithMany("Votes")
                        .HasForeignKey("ValidationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Validation");
                });

            modelBuilder.Entity("Solidarity.Application.Accounts.Account", b =>
                {
                    b.Navigation("AuthenticationMethods");

                    b.Navigation("Campaigns");

                    b.Navigation("Votes");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.Campaign", b =>
                {
                    b.Navigation("ActivatedPaymentMethods");

                    b.Navigation("Expenditures");

                    b.Navigation("Media");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignAllocation", b =>
                {
                    b.Navigation("Campaign")
                        .IsRequired();

                    b.Navigation("Entries");
                });

            modelBuilder.Entity("Solidarity.Application.Campaigns.CampaignValidation", b =>
                {
                    b.Navigation("Campaign")
                        .IsRequired();

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
