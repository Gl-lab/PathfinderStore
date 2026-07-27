using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsumptionMode",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsumptionQuantity",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemInstance_Consumption",
                schema: "inventory",
                table: "ItemInstance",
                sql: "(\"ConsumptionMode\" IS NULL AND \"ConsumptionQuantity\" IS NULL) OR (\"ConsumptionMode\" IN (1, 2, 3) AND \"ConsumptionQuantity\" > 0 AND ((\"IsStackable\" AND \"ConsumptionMode\" IN (2, 3)) OR (NOT \"IsStackable\" AND \"ConsumptionMode\" = 1 AND \"ConsumptionQuantity\" = 1)))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemInstance_Consumption",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "ConsumptionMode",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "ConsumptionQuantity",
                schema: "inventory",
                table: "ItemInstance");
        }
    }
}
