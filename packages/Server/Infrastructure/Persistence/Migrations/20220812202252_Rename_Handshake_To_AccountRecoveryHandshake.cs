using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class Rename_Handshake_To_AccountRecoveryHandshake : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameTable(name: "Handshakes", newName: "AccountRecoveryHandshakes");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameTable(name: "AccountRecoveryHandshakes", newName: "Handshakes");
		}
	}
}
