using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManualShopOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopOffer",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    ItemConfigurationId = table.Column<int>(type: "integer", nullable: true),
                    ItemInstanceKey = table.Column<Guid>(type: "uuid", nullable: true),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceCopper = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOffer", x => x.Id);
                    table.CheckConstraint("CK_ShopOffer_Identity", "\"CampaignId\" > 0 AND \"ShopId\" > 0");
                    table.CheckConstraint("CK_ShopOffer_QuantityPrice", "\"AvailableQuantity\" > 0 AND \"UnitPriceCopper\" >= 0");
                    table.CheckConstraint("CK_ShopOffer_Target", "(\"Kind\" = 1 AND \"ItemConfigurationId\" IS NOT NULL AND \"ItemInstanceKey\" IS NULL) OR (\"Kind\" = 2 AND \"ItemConfigurationId\" IS NULL AND \"ItemInstanceKey\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ShopOffer_Shop_ShopId",
                        column: x => x.ShopId,
                        principalSchema: "commerce",
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopOffer_CampaignId_ShopId_Status",
                schema: "commerce",
                table: "ShopOffer",
                columns: new[] { "CampaignId", "ShopId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ShopOffer_ItemInstanceKey",
                schema: "commerce",
                table: "ShopOffer",
                column: "ItemInstanceKey",
                unique: true,
                filter: "\"Status\" = 1 AND \"ItemInstanceKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOffer_OfferKey",
                schema: "commerce",
                table: "ShopOffer",
                column: "OfferKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopOffer_ShopId",
                schema: "commerce",
                table: "ShopOffer",
                column: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopOffer",
                schema: "commerce");
        }
    }
}
