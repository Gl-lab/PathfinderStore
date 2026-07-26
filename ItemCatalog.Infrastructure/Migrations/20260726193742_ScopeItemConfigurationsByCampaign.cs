using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ScopeItemConfigurationsByCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                schema: "item_catalog",
                table: "ItemConfiguration",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemConfiguration_CampaignId_ItemRevisionId",
                schema: "item_catalog",
                table: "ItemConfiguration",
                columns: new[] { "CampaignId", "ItemRevisionId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemConfiguration_Campaign",
                schema: "item_catalog",
                table: "ItemConfiguration",
                sql: "\"CampaignId\" IS NULL OR \"CampaignId\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemConfiguration_CampaignId_ItemRevisionId",
                schema: "item_catalog",
                table: "ItemConfiguration");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemConfiguration_Campaign",
                schema: "item_catalog",
                table: "ItemConfiguration");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                schema: "item_catalog",
                table: "ItemConfiguration");
        }
    }
}
