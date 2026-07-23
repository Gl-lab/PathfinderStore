using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ShopOffer_QuantityPrice",
                schema: "commerce",
                table: "ShopOffer");

            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                schema: "commerce",
                table: "ShopOffer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "commerce",
                table: "ShopOffer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PurchaseReservation",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReservationKey = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    OfferKey = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerCharacterId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceCopper = table.Column<long>(type: "bigint", nullable: false),
                    TotalPriceCopper = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReservation", x => x.Id);
                    table.CheckConstraint("CK_PurchaseReservation_Identity", "\"CampaignId\" > 0 AND \"BuyerCharacterId\" > 0 AND \"Quantity\" > 0");
                    table.CheckConstraint("CK_PurchaseReservation_Price", "\"UnitPriceCopper\" >= 0 AND \"TotalPriceCopper\" >= 0");
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ShopOffer_QuantityPrice",
                schema: "commerce",
                table: "ShopOffer",
                sql: "\"AvailableQuantity\" > 0 AND \"ReservedQuantity\" >= 0 AND \"ReservedQuantity\" <= \"AvailableQuantity\" AND \"UnitPriceCopper\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReservation_CampaignId_OfferKey_Status",
                schema: "commerce",
                table: "PurchaseReservation",
                columns: new[] { "CampaignId", "OfferKey", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReservation_CampaignId_OperationId",
                schema: "commerce",
                table: "PurchaseReservation",
                columns: new[] { "CampaignId", "OperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReservation_ReservationKey",
                schema: "commerce",
                table: "PurchaseReservation",
                column: "ReservationKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseReservation",
                schema: "commerce");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ShopOffer_QuantityPrice",
                schema: "commerce",
                table: "ShopOffer");

            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                schema: "commerce",
                table: "ShopOffer");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "commerce",
                table: "ShopOffer");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ShopOffer_QuantityPrice",
                schema: "commerce",
                table: "ShopOffer",
                sql: "\"AvailableQuantity\" > 0 AND \"UnitPriceCopper\" >= 0");
        }
    }
}
