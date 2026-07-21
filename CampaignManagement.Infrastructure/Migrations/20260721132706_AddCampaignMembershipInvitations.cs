using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.CampaignManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignMembershipInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignInvitation",
                schema: "campaign_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    InvitedUserId = table.Column<int>(type: "integer", nullable: false),
                    InvitedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RespondedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignInvitation_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "campaign_management",
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitation_CampaignId_InvitedUserId_Status",
                schema: "campaign_management",
                table: "CampaignInvitation",
                columns: new[] { "CampaignId", "InvitedUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitation_InvitedUserId",
                schema: "campaign_management",
                table: "CampaignInvitation",
                column: "InvitedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignInvitation",
                schema: "campaign_management");
        }
    }
}
