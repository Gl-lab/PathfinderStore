using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestockSelectionWeights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsumableWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "PermanentWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "UniqueWeight",
                schema: "commerce",
                table: "RestockPolicyRevision",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RestockPolicyRevision_Weights",
                schema: "commerce",
                table: "RestockPolicyRevision",
                sql: "\"ConsumableWeight\" >= 0 AND \"PermanentWeight\" >= 0 AND \"UniqueWeight\" >= 0 AND (\"ConsumableWeight\" + \"PermanentWeight\" + \"UniqueWeight\") > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RestockPolicyRevision_Weights",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "ConsumableWeight",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "PermanentWeight",
                schema: "commerce",
                table: "RestockPolicyRevision");

            migrationBuilder.DropColumn(
                name: "UniqueWeight",
                schema: "commerce",
                table: "RestockPolicyRevision");
        }
    }
}
