using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class ExtendCampaignAllocation : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AllocationDate",
				table: "Campaigns");

			migrationBuilder.DropColumn(
				name: "CompletionDate",
				table: "Campaigns");

			migrationBuilder.AddColumn<int>(
				name: "AllocationId",
				table: "Campaigns",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "AllocationDestination",
				table: "CampaignPaymentMethods",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.CreateTable(
				name: "CampaignAllocation",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CampaignAllocation", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "CampaignAllocationEntry",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CampaignAllocationId = table.Column<int>(type: "int", nullable: false),
					Type = table.Column<int>(type: "int", nullable: false),
					PaymentMethodIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					CreatorId = table.Column<int>(type: "int", nullable: true),
					Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
					LastModifierId = table.Column<int>(type: "int", nullable: true),
					LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CampaignAllocationEntry", x => x.Id);
					table.ForeignKey(
						name: "FK_CampaignAllocationEntry_CampaignAllocation_CampaignAllocationId",
						column: x => x.CampaignAllocationId,
						principalTable: "CampaignAllocation",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Campaigns_AllocationId",
				table: "Campaigns",
				column: "AllocationId",
				unique: true,
				filter: "[AllocationId] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_CampaignAllocationEntry_CampaignAllocationId",
				table: "CampaignAllocationEntry",
				column: "CampaignAllocationId");

			migrationBuilder.AddForeignKey(
				name: "FK_Campaigns_CampaignAllocation_AllocationId",
				table: "Campaigns",
				column: "AllocationId",
				principalTable: "CampaignAllocation",
				principalColumn: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Campaigns_CampaignAllocation_AllocationId",
				table: "Campaigns");

			migrationBuilder.DropTable(
				name: "CampaignAllocationEntry");

			migrationBuilder.DropTable(
				name: "CampaignAllocation");

			migrationBuilder.DropIndex(
				name: "IX_Campaigns_AllocationId",
				table: "Campaigns");

			migrationBuilder.DropColumn(
				name: "AllocationId",
				table: "Campaigns");

			migrationBuilder.DropColumn(
				name: "AllocationDestination",
				table: "CampaignPaymentMethods");

			migrationBuilder.AddColumn<DateTime>(
				name: "AllocationDate",
				table: "Campaigns",
				type: "datetime2",
				nullable: true);

			migrationBuilder.AddColumn<DateTime>(
				name: "CompletionDate",
				table: "Campaigns",
				type: "datetime2",
				nullable: true);
		}
	}
}
