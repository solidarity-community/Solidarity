using Microsoft.EntityFrameworkCore.Migrations;

namespace Solidarity.Migrations;

public partial class Rename_DonationChannel_To_CampaignDonationChannel : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.RenameTable("DonationChannels", "CampaignDonationChannels");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.RenameTable("CampaignDonationChannels", "DonationChannels");
	}
}