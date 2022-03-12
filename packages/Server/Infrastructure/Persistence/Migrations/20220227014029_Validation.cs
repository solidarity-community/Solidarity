using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
    public partial class Validation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_Validations_ValidationId",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_ValidationId",
                table: "Campaigns");

            migrationBuilder.AlterColumn<int>(
                name: "ValidationId",
                table: "Campaigns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ValidationId",
                table: "Campaigns",
                column: "ValidationId",
                unique: true,
                filter: "[ValidationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_Validations_ValidationId",
                table: "Campaigns",
                column: "ValidationId",
                principalTable: "Validations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_Validations_ValidationId",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_ValidationId",
                table: "Campaigns");

            migrationBuilder.AlterColumn<int>(
                name: "ValidationId",
                table: "Campaigns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ValidationId",
                table: "Campaigns",
                column: "ValidationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_Validations_ValidationId",
                table: "Campaigns",
                column: "ValidationId",
                principalTable: "Validations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
