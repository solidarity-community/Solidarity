using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class PaymentMethods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationChannels_Accounts_DonorId",
                table: "DonationChannels");

            migrationBuilder.DropTable(
                name: "CryptoMnemonics");

            migrationBuilder.DropIndex(
                name: "IX_DonationChannels_DonorId",
                table: "DonationChannels");

            migrationBuilder.DropColumn(
                name: "DonorId",
                table: "DonationChannels");

            migrationBuilder.RenameColumn(
                name: "WalletAddress",
                table: "DonationChannels",
                newName: "PaymentMethodIdentifier");

            migrationBuilder.CreateTable(
                name: "PaymentMethodKeys",
                columns: table => new
                {
                    PaymentMethodIdentifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifierId = table.Column<int>(type: "int", nullable: false),
                    LastModification = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodKeys", x => x.PaymentMethodIdentifier);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentMethodKeys");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodIdentifier",
                table: "DonationChannels",
                newName: "WalletAddress");

            migrationBuilder.AddColumn<int>(
                name: "DonorId",
                table: "DonationChannels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CryptoMnemonics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    LastModification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifierId = table.Column<int>(type: "int", nullable: false),
                    Mnemonic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoMnemonics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationChannels_DonorId",
                table: "DonationChannels",
                column: "DonorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationChannels_Accounts_DonorId",
                table: "DonationChannels",
                column: "DonorId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
