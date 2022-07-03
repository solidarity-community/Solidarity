using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solidarity.Infrastructure.Persistence.Migrations
{
	public partial class RenameValidationAndVoteToCampaignValidationAndCampaignValidationVote : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameTable(name: "Validation", newName: "CampaignValidation");
			migrationBuilder.RenameTable(name: "Vote", newName: "CampaignValidationVote");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameTable(name: "CampaignValidation", newName: "Validation");
			migrationBuilder.RenameTable(name: "CampaignValidationVote", newName: "Vote");
		}
	}
}
