using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.CampaignManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignPartyStorageRoot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignPartyStorage",
                schema: "campaign_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignPartyId = table.Column<int>(type: "integer", nullable: false),
                    AccessPolicy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignPartyStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignPartyStorage_CampaignParty_CampaignPartyId",
                        column: x => x.CampaignPartyId,
                        principalSchema: "campaign_management",
                        principalTable: "CampaignParty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO campaign_management."CampaignPartyStorage"
                    ( "CampaignPartyId", "AccessPolicy", "CreatedAtUtc" )
                SELECT "Id", 0, "CreatedAtUtc"
                FROM campaign_management."CampaignParty";
                """ );

            migrationBuilder.CreateIndex(
                name: "IX_CampaignPartyStorage_CampaignPartyId",
                schema: "campaign_management",
                table: "CampaignPartyStorage",
                column: "CampaignPartyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignPartyStorage",
                schema: "campaign_management");
        }
    }
}
