using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ScopeItemDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemDefinition_Key",
                schema: "item_catalog",
                table: "ItemDefinition" );

            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                schema: "item_catalog",
                table: "ItemDefinition",
                type: "integer",
                nullable: true );

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                schema: "item_catalog",
                table: "ItemDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 1 );

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_CampaignId_Key",
                schema: "item_catalog",
                table: "ItemDefinition",
                columns: new[] { "CampaignId", "Key" },
                unique: true,
                filter: "\"Scope\" = 2" );

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_Key",
                schema: "item_catalog",
                table: "ItemDefinition",
                column: "Key",
                unique: true,
                filter: "\"Scope\" = 1" );

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemDefinition_Scope",
                schema: "item_catalog",
                table: "ItemDefinition",
                sql: "(\"Scope\" = 1 AND \"CampaignId\" IS NULL) OR (\"Scope\" = 2 AND \"CampaignId\" > 0)" );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemDefinition_CampaignId_Key",
                schema: "item_catalog",
                table: "ItemDefinition" );

            migrationBuilder.DropIndex(
                name: "IX_ItemDefinition_Key",
                schema: "item_catalog",
                table: "ItemDefinition" );

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemDefinition_Scope",
                schema: "item_catalog",
                table: "ItemDefinition" );

            migrationBuilder.DropColumn(
                name: "CampaignId",
                schema: "item_catalog",
                table: "ItemDefinition" );

            migrationBuilder.DropColumn(
                name: "Scope",
                schema: "item_catalog",
                table: "ItemDefinition" );

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_Key",
                schema: "item_catalog",
                table: "ItemDefinition",
                column: "Key",
                unique: true );
        }
    }
}