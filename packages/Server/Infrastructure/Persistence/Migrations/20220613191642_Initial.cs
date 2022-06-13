﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class Initial : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Accounts",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
					PublicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Accounts", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "PaymentMethodKeys",
				columns: table => new
				{
					PaymentMethodIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Id = table.Column<int>(type: "int", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PaymentMethodKeys", x => x.PaymentMethodIdentifier);
				});

			migrationBuilder.CreateTable(
				name: "Validations",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Validations", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AuthenticationMethods",
				columns: table => new
				{
					AccountId = table.Column<int>(type: "int", nullable: false),
					Type = table.Column<int>(type: "int", nullable: false),
					Salt = table.Column<int>(type: "int", nullable: false),
					Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Id = table.Column<int>(type: "int", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AuthenticationMethods", x => new { x.AccountId, x.Type, x.Salt });
					table.ForeignKey(
						name: "FK_AuthenticationMethods_Accounts_AccountId",
						column: x => x.AccountId,
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Handshakes",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					AccountId = table.Column<int>(type: "int", nullable: false),
					Phrase = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Handshakes", x => x.Id);
					table.ForeignKey(
						name: "FK_Handshakes_Accounts_AccountId",
						column: x => x.AccountId,
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Identities",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					AccountId = table.Column<int>(type: "int", nullable: false),
					FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Identities", x => x.Id);
					table.ForeignKey(
						name: "FK_Identities_Accounts_AccountId",
						column: x => x.AccountId,
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Campaigns",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Location = table.Column<Geometry>(type: "geography", nullable: false),
					TargetAllocationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					AllocationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
					CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
					ValidationId = table.Column<int>(type: "int", nullable: true),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Campaigns", x => x.Id);
					table.ForeignKey(
						name: "FK_Campaigns_Accounts_CreatorId",
						column: x => x.CreatorId,
						principalTable: "Accounts",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Campaigns_Validations_ValidationId",
						column: x => x.ValidationId,
						principalTable: "Validations",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Votes",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ValidationId = table.Column<int>(type: "int", nullable: false),
					AccountId = table.Column<int>(type: "int", nullable: false),
					Value = table.Column<bool>(type: "bit", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Votes", x => x.Id);
					table.ForeignKey(
						name: "FK_Votes_Accounts_AccountId",
						column: x => x.AccountId,
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Votes_Validations_ValidationId",
						column: x => x.ValidationId,
						principalTable: "Validations",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "CampaignExpenditures",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CampaignId = table.Column<int>(type: "int", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false),
					UnitPrice = table.Column<long>(type: "bigint", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CampaignExpenditures", x => x.Id);
					table.ForeignKey(
						name: "FK_CampaignExpenditures_Campaigns_CampaignId",
						column: x => x.CampaignId,
						principalTable: "Campaigns",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "CampaignMedia",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CampaignId = table.Column<int>(type: "int", nullable: false),
					Type = table.Column<int>(type: "int", nullable: false),
					Uri = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CampaignMedia", x => x.Id);
					table.ForeignKey(
						name: "FK_CampaignMedia_Campaigns_CampaignId",
						column: x => x.CampaignId,
						principalTable: "Campaigns",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "CampaignPaymentMethods",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Identifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CampaignId = table.Column<int>(type: "int", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CampaignPaymentMethods", x => x.Id);
					table.ForeignKey(
						name: "FK_CampaignPaymentMethods_Campaigns_CampaignId",
						column: x => x.CampaignId,
						principalTable: "Campaigns",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Accounts_Username",
				table: "Accounts",
				column: "Username",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_CampaignExpenditures_CampaignId",
				table: "CampaignExpenditures",
				column: "CampaignId");

			migrationBuilder.CreateIndex(
				name: "IX_CampaignMedia_CampaignId",
				table: "CampaignMedia",
				column: "CampaignId");

			migrationBuilder.CreateIndex(
				name: "IX_CampaignPaymentMethods_CampaignId",
				table: "CampaignPaymentMethods",
				column: "CampaignId");

			migrationBuilder.CreateIndex(
				name: "IX_Campaigns_CreatorId",
				table: "Campaigns",
				column: "CreatorId");

			migrationBuilder.CreateIndex(
				name: "IX_Campaigns_ValidationId",
				table: "Campaigns",
				column: "ValidationId",
				unique: true,
				filter: "[ValidationId] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Handshakes_AccountId",
				table: "Handshakes",
				column: "AccountId");

			migrationBuilder.CreateIndex(
				name: "IX_Identities_AccountId",
				table: "Identities",
				column: "AccountId",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Votes_AccountId",
				table: "Votes",
				column: "AccountId");

			migrationBuilder.CreateIndex(
				name: "IX_Votes_ValidationId",
				table: "Votes",
				column: "ValidationId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AuthenticationMethods");

			migrationBuilder.DropTable(
				name: "CampaignExpenditures");

			migrationBuilder.DropTable(
				name: "CampaignMedia");

			migrationBuilder.DropTable(
				name: "CampaignPaymentMethods");

			migrationBuilder.DropTable(
				name: "Handshakes");

			migrationBuilder.DropTable(
				name: "Identities");

			migrationBuilder.DropTable(
				name: "PaymentMethodKeys");

			migrationBuilder.DropTable(
				name: "Votes");

			migrationBuilder.DropTable(
				name: "Campaigns");

			migrationBuilder.DropTable(
				name: "Accounts");

			migrationBuilder.DropTable(
				name: "Validations");
		}
	}
}
