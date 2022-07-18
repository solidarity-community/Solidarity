using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class Rename_Identity_To_AccountProfile : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameTable(name: "Identities", newName: "AccountProfiles");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameTable(name: "AccountProfiles", newName: "Identities");
		}
	}
}
