using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class SetupCompositeKeyForVote : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
				name: "PK_Votes",
				table: "Votes");

			migrationBuilder.DropIndex(
				name: "IX_Votes_ValidationId",
				table: "Votes");

			migrationBuilder.AlterColumn<int>(
				name: "Id",
				table: "Votes",
				type: "int",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int")
				.OldAnnotation("SqlServer:Identity", "1, 1");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Votes",
				table: "Votes",
				columns: new[] { "ValidationId", "AccountId" });
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
				name: "PK_Votes",
				table: "Votes");

			migrationBuilder.AlterColumn<int>(
				name: "Id",
				table: "Votes",
				type: "int",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int")
				.Annotation("SqlServer:Identity", "1, 1");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Votes",
				table: "Votes",
				column: "Id");

			migrationBuilder.CreateIndex(
				name: "IX_Votes_ValidationId",
				table: "Votes",
				column: "ValidationId");
		}
	}
}
