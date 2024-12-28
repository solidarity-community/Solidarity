using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Squash : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(name: "Accounts",
			columns: table => new
			{
				Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
				Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
				PublicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
				Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
				BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
				Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Accounts", x => x.Id);
			});
		migrationBuilder.CreateTable(name: "CampaignAllocation",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1")
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignAllocation", x => x.Id);
		});
		migrationBuilder.CreateTable(name: "CampaignValidation",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			Expiration = table.Column<DateTime>(type: "datetime2", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignValidation", x => x.Id);
		});
		migrationBuilder.CreateTable(name: "PaymentMethodKeys",
		columns: table => new
		{
			PaymentMethodIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
			Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Id = table.Column<int>(type: "int", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_PaymentMethodKeys", x => x.PaymentMethodIdentifier);
		});
		migrationBuilder.CreateTable(name: "AccountRecoveryHandshakes",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			AccountId = table.Column<int>(type: "int", nullable: false),
			Phrase = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Expiration = table.Column<DateTime>(type: "datetime2", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_AccountRecoveryHandshakes", x => x.Id);
			table.ForeignKey(name: "FK_AccountRecoveryHandshakes_Accounts_AccountId", column: x => x.AccountId,
				principalTable: "Accounts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable(name: "Audits",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			AccountId = table.Column<int>(type: "int", nullable: true),
			Date = table.Column<DateTime>(type: "datetime2", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_Audits", x => x.Id);
			table.ForeignKey(name: "FK_Audits_Accounts_AccountId", column: x => x.AccountId,
				principalTable: "Accounts",
				principalColumn: "Id");
		});
		migrationBuilder.CreateTable(name: "CampaignAllocationEntry",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			CampaignAllocationId = table.Column<int>(type: "int", nullable: false),
			Type = table.Column<int>(type: "int", nullable: false),
			PaymentMethodIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignAllocationEntry", x => x.Id);
			table.ForeignKey(name: "FK_CampaignAllocationEntry_CampaignAllocation_CampaignAllocationId", column: x => x.CampaignAllocationId,
				principalTable: "CampaignAllocation",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable(name: "Campaigns",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			CreatorId = table.Column<int>(type: "int", nullable: false),
			Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
			Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Location = table.Column<Geometry>(type: "geography", nullable: false),
			ValidationId = table.Column<int>(type: "int", nullable: true),
			AllocationId = table.Column<int>(type: "int", nullable: true)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_Campaigns", x => x.Id);
			table.ForeignKey(name: "FK_Campaigns_Accounts_CreatorId", column: x => x.CreatorId,
				principalTable: "Accounts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
			table.ForeignKey(name: "FK_Campaigns_CampaignAllocation_AllocationId", column: x => x.AllocationId,
				principalTable: "CampaignAllocation",
				principalColumn: "Id");
			table.ForeignKey(name: "FK_Campaigns_CampaignValidation_ValidationId", column: x => x.ValidationId,
				principalTable: "CampaignValidation",
				principalColumn: "Id");
		});
		migrationBuilder.CreateTable(name: "CampaignValidationVote",
		columns: table => new
		{
			ValidationId = table.Column<int>(type: "int", nullable: false),
			AccountId = table.Column<int>(type: "int", nullable: false),
			Value = table.Column<bool>(type: "bit", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignValidationVote", x => new { x.ValidationId, x.AccountId });
			table.ForeignKey(name: "FK_CampaignValidationVote_Accounts_AccountId", column: x => x.AccountId,
				principalTable: "Accounts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
			table.ForeignKey(name: "FK_CampaignValidationVote_CampaignValidation_ValidationId", column: x => x.ValidationId,
				principalTable: "CampaignValidation",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable(name: "AuditChange",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			EntityKey = table.Column<int>(type: "int", nullable: true),
			AuditId = table.Column<int>(type: "int", nullable: false),
			State = table.Column<int>(type: "int", nullable: false),
			Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_AuditChange", x => x.Id);
			table.ForeignKey(name: "FK_AuditChange_Audits_AuditId", column: x => x.AuditId,
				principalTable: "Audits",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable(name: "CampaignExpenditure",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			CampaignId = table.Column<int>(type: "int", nullable: false),
			Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
			Quantity = table.Column<int>(type: "int", nullable: false),
			UnitPrice = table.Column<long>(type: "bigint", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignExpenditure", x => x.Id);
			table.ForeignKey(name: "FK_CampaignExpenditure_Campaigns_CampaignId", column: x => x.CampaignId,
				principalTable: "Campaigns",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable(name: "CampaignMedia",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			CampaignId = table.Column<int>(type: "int", nullable: false),
			Type = table.Column<int>(type: "int", nullable: false),
			Uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignMedia", x => x.Id);
			table.ForeignKey(name: "FK_CampaignMedia_Campaigns_CampaignId", column: x => x.CampaignId,
				principalTable: "Campaigns",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable(name: "CampaignPaymentMethod",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
			Identifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
			CampaignId = table.Column<int>(type: "int", nullable: false),
			AllocationDestination = table.Column<string>(type: "nvarchar(max)", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_CampaignPaymentMethod", x => x.Id);
			table.ForeignKey(name: "FK_CampaignPaymentMethod_Campaigns_CampaignId", column: x => x.CampaignId,
				principalTable: "Campaigns",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});
		migrationBuilder.CreateIndex(name: "IX_AccountRecoveryHandshakes_AccountId", table: "AccountRecoveryHandshakes", column: "AccountId");
		migrationBuilder.CreateIndex(name: "IX_Accounts_Username", table: "Accounts", column: "Username", unique: true);
		migrationBuilder.CreateIndex(name: "IX_AuditChange_AuditId", table: "AuditChange", column: "AuditId");
		migrationBuilder.CreateIndex(name: "IX_Audits_AccountId", table: "Audits", column: "AccountId");
		migrationBuilder.CreateIndex(name: "IX_CampaignAllocationEntry_CampaignAllocationId", table: "CampaignAllocationEntry", column: "CampaignAllocationId");
		migrationBuilder.CreateIndex(name: "IX_CampaignExpenditure_CampaignId", table: "CampaignExpenditure", column: "CampaignId");
		migrationBuilder.CreateIndex(name: "IX_CampaignMedia_CampaignId", table: "CampaignMedia", column: "CampaignId");
		migrationBuilder.CreateIndex(name: "IX_CampaignPaymentMethod_CampaignId", table: "CampaignPaymentMethod", column: "CampaignId");
		migrationBuilder.CreateIndex(name: "IX_Campaigns_AllocationId", table: "Campaigns", column: "AllocationId", unique: true, filter: "[AllocationId] IS NOT NULL");
		migrationBuilder.CreateIndex(name: "IX_Campaigns_CreatorId", table: "Campaigns", column: "CreatorId");
		migrationBuilder.CreateIndex(name: "IX_Campaigns_ValidationId", table: "Campaigns", column: "ValidationId", unique: true, filter: "[ValidationId] IS NOT NULL");
		migrationBuilder.CreateIndex(name: "IX_CampaignValidationVote_AccountId", table: "CampaignValidationVote", column: "AccountId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(name: "AccountRecoveryHandshakes");
		migrationBuilder.DropTable(name: "AuditChange");
		migrationBuilder.DropTable(name: "CampaignAllocationEntry");
		migrationBuilder.DropTable(name: "CampaignExpenditure");
		migrationBuilder.DropTable(name: "CampaignMedia");
		migrationBuilder.DropTable(name: "CampaignPaymentMethod");
		migrationBuilder.DropTable(name: "CampaignValidationVote");
		migrationBuilder.DropTable(name: "PaymentMethodKeys");
		migrationBuilder.DropTable(name: "Audits");
		migrationBuilder.DropTable(name: "Campaigns");
		migrationBuilder.DropTable(name: "Accounts");
		migrationBuilder.DropTable(name: "CampaignAllocation");
		migrationBuilder.DropTable(name: "CampaignValidation");
	}
}