using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRestockGenerationDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermanentWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "MaximumItemLevel",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 20);

            migrationBuilder.AlterColumn<int>(
                name: "ConsumableWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "AllowedRarities",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 15);

            migrationBuilder.AlterColumn<int>(
                name: "AllowedCategories",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 511);

            migrationBuilder.AlterColumn<int>(
                name: "AllowedAccess",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermanentWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MaximumItemLevel",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 20,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ConsumableWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AllowedRarities",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 15,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AllowedCategories",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 511,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AllowedAccess",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
