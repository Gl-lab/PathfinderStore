using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.CampaignManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignParties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignParty",
                schema: "campaign_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ArchivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignParty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignParty_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "campaign_management",
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignPartyCharacter",
                schema: "campaign_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignPartyId = table.Column<int>(type: "integer", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ControlledByUserId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPartyCharacter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignPartyCharacter_CampaignParty_CampaignPartyId",
                        column: x => x.CampaignPartyId,
                        principalSchema: "campaign_management",
                        principalTable: "CampaignParty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignParty_CampaignId",
                schema: "campaign_management",
                table: "CampaignParty",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPartyCharacter_CampaignPartyId_CharacterId",
                schema: "campaign_management",
                table: "CampaignPartyCharacter",
                columns: new[] { "CampaignPartyId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPartyCharacter_CharacterId",
                schema: "campaign_management",
                table: "CampaignPartyCharacter",
                column: "CharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignPartyCharacter",
                schema: "campaign_management");

            migrationBuilder.DropTable(
                name: "CampaignParty",
                schema: "campaign_management");
        }
    }
}
