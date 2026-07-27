using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestockPolicyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllowedAccess",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "AllowedCategories",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 511);

            migrationBuilder.AddColumn<int>(
                name: "AllowedRarities",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 15);

            migrationBuilder.AddColumn<long>(
                name: "BudgetCopper",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "MaximumItemLevel",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<int>(
                name: "MinimumItemLevel",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RestockPolicyRevision_Constraints",
                schema: "commerce",
                table: "RestockPolicyRevision",
                sql: "\"MinimumItemLevel\" >= 0 AND \"MaximumItemLevel\" >= \"MinimumItemLevel\" AND \"MaximumItemLevel\" <= 30 AND \"BudgetCopper\" >= 0 AND \"AllowedRarities\" > 0 AND \"AllowedAccess\" > 0 AND \"AllowedCategories\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RestockPolicyRevision_Constraints",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "AllowedAccess",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "AllowedCategories",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "AllowedRarities",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "BudgetCopper",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "MaximumItemLevel",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "MinimumItemLevel",
                schema: "commerce",
                table: "RestockPolicyRevision");
        }
    }
}
