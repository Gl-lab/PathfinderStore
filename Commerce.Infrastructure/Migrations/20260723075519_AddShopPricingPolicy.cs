using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShopPricingPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuybackPricePercent",
                schema: "commerce",
                table: "Shop",
                type: "integer",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.AddColumn<int>(
                name: "CatalogPricePercent",
                schema: "commerce",
                table: "Shop",
                type: "integer",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<int>(
                name: "PricingPolicyVersion",
                schema: "commerce",
                table: "Shop",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuybackPricePercent",
                schema: "commerce",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "CatalogPricePercent",
                schema: "commerce",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "PricingPolicyVersion",
                schema: "commerce",
                table: "Shop");
        }
    }
}
