using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDurability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrokenThreshold",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHitPoints",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Hardness",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumHitPoints",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemInstance_Durability",
                schema: "inventory",
                table: "ItemInstance",
                sql: "(\"Hardness\" IS NULL AND \"MaximumHitPoints\" IS NULL AND \"CurrentHitPoints\" IS NULL AND \"BrokenThreshold\" IS NULL) OR (NOT \"IsStackable\" AND \"Hardness\" >= 0 AND \"MaximumHitPoints\" > 0 AND \"CurrentHitPoints\" >= 0 AND \"CurrentHitPoints\" <= \"MaximumHitPoints\" AND \"BrokenThreshold\" > 0 AND \"BrokenThreshold\" <= \"MaximumHitPoints\" AND ((\"CurrentHitPoints\" = 0 AND \"Quantity\" = 0) OR (\"CurrentHitPoints\" > 0 AND \"Quantity\" = 1)))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemInstance_Durability",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "BrokenThreshold",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "CurrentHitPoints",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "Hardness",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "MaximumHitPoints",
                schema: "inventory",
                table: "ItemInstance");
        }
    }
}
