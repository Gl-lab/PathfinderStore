using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteShopTrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PurchasedItemInstanceKey",
                schema: "commerce",
                table: "PurchaseReservation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShopSale",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleKey = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    SellerCharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemInstanceKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemConfigurationId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceCopper = table.Column<long>(type: "bigint", nullable: false),
                    TotalPriceCopper = table.Column<long>(type: "bigint", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopSale", x => x.Id);
                    table.CheckConstraint("CK_ShopSale_Identity", "\"CampaignId\" > 0 AND \"ShopId\" > 0 AND \"SellerCharacterId\" > 0 AND \"ItemConfigurationId\" > 0");
                    table.CheckConstraint("CK_ShopSale_Price", "\"Quantity\" > 0 AND \"UnitPriceCopper\" >= 0 AND \"TotalPriceCopper\" >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopSale_CampaignId_OperationId",
                schema: "commerce",
                table: "ShopSale",
                columns: new[] { "CampaignId", "OperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopSale_SaleKey",
                schema: "commerce",
                table: "ShopSale",
                column: "SaleKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopSale",
                schema: "commerce");

            migrationBuilder.DropColumn(
                name: "PurchasedItemInstanceKey",
                schema: "commerce",
                table: "PurchaseReservation");
        }
    }
}
