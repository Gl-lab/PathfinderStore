using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChargeRecoveryRule",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentCharges",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultActivationCost",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumCharges",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemInstance_Charges",
                schema: "inventory",
                table: "ItemInstance",
                sql: "(\"MaximumCharges\" IS NULL AND \"CurrentCharges\" IS NULL AND \"DefaultActivationCost\" IS NULL AND \"ChargeRecoveryRule\" IS NULL) OR (\"MaximumCharges\" > 0 AND \"CurrentCharges\" >= 0 AND \"CurrentCharges\" <= \"MaximumCharges\" AND \"DefaultActivationCost\" > 0 AND \"DefaultActivationCost\" <= \"MaximumCharges\" AND \"ChargeRecoveryRule\" IN (1, 2, 3))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemInstance_Charges",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "ChargeRecoveryRule",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "CurrentCharges",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DefaultActivationCost",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "MaximumCharges",
                schema: "inventory",
                table: "ItemInstance");
        }
    }
}
