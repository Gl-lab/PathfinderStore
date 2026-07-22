using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartyGifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartyGift",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GiftKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    PartyId = table.Column<int>(type: "integer", nullable: false),
                    SourceCharacterId = table.Column<int>(type: "integer", nullable: false),
                    DestinationCharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemInstanceKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpectedItemVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AcceptedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AcceptanceOperationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyGift", x => x.Id);
                    table.CheckConstraint("CK_PartyGift_State", "\"CampaignId\" > 0 AND \"PartyId\" > 0 AND \"SourceCharacterId\" > 0 AND \"DestinationCharacterId\" > 0 AND \"ExpectedItemVersion\" >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartyGift_CampaignId_PartyId_Status",
                schema: "inventory",
                table: "PartyGift",
                columns: new[] { "CampaignId", "PartyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PartyGift_GiftKey",
                schema: "inventory",
                table: "PartyGift",
                column: "GiftKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyGift_ItemInstanceKey",
                schema: "inventory",
                table: "PartyGift",
                column: "ItemInstanceKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartyGift",
                schema: "inventory");
        }
    }
}
