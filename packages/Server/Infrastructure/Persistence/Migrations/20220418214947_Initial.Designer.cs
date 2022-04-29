﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Solidarity.Infrastructure.Persistence;

#nullable disable

namespace Solidarity.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220418214947_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Solidarity.Domain.Models.Account", b =>
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

            modelBuilder.Entity("Solidarity.Domain.Models.AuthenticationMethod", b =>
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

            modelBuilder.Entity("Solidarity.Domain.Models.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("Completion")
                        .HasColumnType("datetime2");

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

                    b.HasIndex("CreatorId");

                    b.HasIndex("ValidationId")
                        .IsUnique()
                        .HasFilter("[ValidationId] IS NOT NULL");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.CampaignExpenditure", b =>
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

            modelBuilder.Entity("Solidarity.Domain.Models.CampaignMedia", b =>
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

            modelBuilder.Entity("Solidarity.Domain.Models.DonationChannel", b =>
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

                    b.Property<string>("PaymentMethodIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("DonationChannels");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Handshake", b =>
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

                    b.ToTable("Handshakes");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Identity", b =>
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

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.PaymentMethodKey", b =>
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

            modelBuilder.Entity("Solidarity.Domain.Models.Validation", b =>
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

                    b.ToTable("Validations");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Vote", b =>
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

                    b.Property<DateTime>("LastModification")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastModifierId")
                        .HasColumnType("int");

                    b.Property<int>("ValidationId")
                        .HasColumnType("int");

                    b.Property<bool>("Value")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ValidationId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.PasswordAuthentication", b =>
                {
                    b.HasBaseType("Solidarity.Domain.Models.AuthenticationMethod");

                    b.HasDiscriminator().HasValue(0);
                });

            modelBuilder.Entity("Solidarity.Domain.Models.AuthenticationMethod", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Account", "Account")
                        .WithMany("AuthenticationMethods")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Campaign", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Account", "Creator")
                        .WithMany("Campaigns")
                        .HasForeignKey("CreatorId");

                    b.HasOne("Solidarity.Domain.Models.Validation", "Validation")
                        .WithOne("Campaign")
                        .HasForeignKey("Solidarity.Domain.Models.Campaign", "ValidationId");

                    b.Navigation("Creator");

                    b.Navigation("Validation");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.CampaignExpenditure", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Campaign", null)
                        .WithMany("Expenditures")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Solidarity.Domain.Models.CampaignMedia", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Campaign", null)
                        .WithMany("Media")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Solidarity.Domain.Models.DonationChannel", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Campaign", "Campaign")
                        .WithMany("DonationChannels")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Handshake", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Identity", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Account", "Account")
                        .WithOne()
                        .HasForeignKey("Solidarity.Domain.Models.Identity", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Vote", b =>
                {
                    b.HasOne("Solidarity.Domain.Models.Account", "Account")
                        .WithMany("Votes")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Solidarity.Domain.Models.Validation", "Validation")
                        .WithMany("Votes")
                        .HasForeignKey("ValidationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Validation");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Account", b =>
                {
                    b.Navigation("AuthenticationMethods");

                    b.Navigation("Campaigns");

                    b.Navigation("Votes");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Campaign", b =>
                {
                    b.Navigation("DonationChannels");

                    b.Navigation("Expenditures");

                    b.Navigation("Media");
                });

            modelBuilder.Entity("Solidarity.Domain.Models.Validation", b =>
                {
                    b.Navigation("Campaign")
                        .IsRequired();

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}