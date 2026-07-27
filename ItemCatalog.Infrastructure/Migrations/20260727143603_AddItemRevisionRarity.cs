using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemRevisionRarity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rarity",
                schema: "item_catalog",
                table: "ItemRevision",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rarity",
                schema: "item_catalog",
                table: "ItemRevision");
        }
    }
}
