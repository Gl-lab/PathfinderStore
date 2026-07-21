using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CampaignManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HardenCampaignInvariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CampaignParty_CampaignId",
                schema: "campaign_management",
                table: "CampaignParty");

            migrationBuilder.DropIndex(
                name: "IX_CampaignInvitation_CampaignId_InvitedUserId_Status",
                schema: "campaign_management",
                table: "CampaignInvitation");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignParty_CampaignId",
                schema: "campaign_management",
                table: "CampaignParty",
                column: "CampaignId",
                unique: true,
                filter: "\"Status\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitation_CampaignId_InvitedUserId",
                schema: "campaign_management",
                table: "CampaignInvitation",
                columns: new[] { "CampaignId", "InvitedUserId" },
                unique: true,
                filter: "\"Status\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CampaignParty_CampaignId",
                schema: "campaign_management",
                table: "CampaignParty");

            migrationBuilder.DropIndex(
                name: "IX_CampaignInvitation_CampaignId_InvitedUserId",
                schema: "campaign_management",
                table: "CampaignInvitation");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignParty_CampaignId",
                schema: "campaign_management",
                table: "CampaignParty",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitation_CampaignId_InvitedUserId_Status",
                schema: "campaign_management",
                table: "CampaignInvitation",
                columns: new[] { "CampaignId", "InvitedUserId", "Status" });
        }
    }
}
