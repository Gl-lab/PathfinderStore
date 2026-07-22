using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartyExchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReservationKey",
                schema: "inventory",
                table: "ItemInstance",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartyExchange",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExchangeKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    PartyId = table.Column<int>(type: "integer", nullable: false),
                    InitiatorCharacterId = table.Column<int>(type: "integer", nullable: false),
                    CounterpartyCharacterId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyExchange", x => x.Id);
                    table.CheckConstraint("CK_PartyExchange_State", "\"CampaignId\" > 0 AND \"PartyId\" > 0 AND \"InitiatorCharacterId\" > 0 AND \"CounterpartyCharacterId\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "PartyExchangeLine",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartyExchangeId = table.Column<int>(type: "integer", nullable: false),
                    FromCharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemInstanceKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpectedItemVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyExchangeLine", x => x.Id);
                    table.CheckConstraint("CK_PartyExchangeLine_State", "\"FromCharacterId\" > 0 AND \"ExpectedItemVersion\" >= 0");
                    table.ForeignKey(
                        name: "FK_PartyExchangeLine_PartyExchange_PartyExchangeId",
                        column: x => x.PartyExchangeId,
                        principalSchema: "inventory",
                        principalTable: "PartyExchange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartyExchange_ExchangeKey",
                schema: "inventory",
                table: "PartyExchange",
                column: "ExchangeKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyExchangeLine_PartyExchangeId_ItemInstanceKey",
                schema: "inventory",
                table: "PartyExchangeLine",
                columns: new[] { "PartyExchangeId", "ItemInstanceKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartyExchangeLine",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "PartyExchange",
                schema: "inventory");

            migrationBuilder.DropColumn(
                name: "ReservationKey",
                schema: "inventory",
                table: "ItemInstance");
        }
    }
}
