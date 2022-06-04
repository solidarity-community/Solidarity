using Microsoft.EntityFrameworkCore.Migrations;

namespace Solidarity.Migrations;

public partial class Required_Campaign_Target_Date : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<DateTime>(
			name: "TargetDate",
			table: "Campaigns",
			type: "datetime2",
			nullable: false,
			defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "TargetDate",
			table: "Campaigns");
	}
}