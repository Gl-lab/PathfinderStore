using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemRevisionLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedAtUtc",
                schema: "item_catalog",
                table: "ItemRevision",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RetiredAtUtc",
                schema: "item_catalog",
                table: "ItemRevision",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "item_catalog",
                table: "ItemRevision",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ItemRevision_ItemDefinitionId",
                schema: "item_catalog",
                table: "ItemRevision",
                column: "ItemDefinitionId",
                unique: true,
                filter: "\"Status\" = 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemRevision_Lifecycle",
                schema: "item_catalog",
                table: "ItemRevision",
                sql: "(\"Status\" = 1 AND \"PublishedAtUtc\" IS NULL AND \"RetiredAtUtc\" IS NULL) OR (\"Status\" = 2 AND \"PublishedAtUtc\" IS NOT NULL AND \"RetiredAtUtc\" IS NULL) OR (\"Status\" = 3 AND \"PublishedAtUtc\" IS NOT NULL AND \"RetiredAtUtc\" IS NOT NULL AND \"RetiredAtUtc\" >= \"PublishedAtUtc\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemRevision_ItemDefinitionId",
                schema: "item_catalog",
                table: "ItemRevision");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemRevision_Lifecycle",
                schema: "item_catalog",
                table: "ItemRevision");

            migrationBuilder.DropColumn(
                name: "PublishedAtUtc",
                schema: "item_catalog",
                table: "ItemRevision");

            migrationBuilder.DropColumn(
                name: "RetiredAtUtc",
                schema: "item_catalog",
                table: "ItemRevision");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "item_catalog",
                table: "ItemRevision");
        }
    }
}
