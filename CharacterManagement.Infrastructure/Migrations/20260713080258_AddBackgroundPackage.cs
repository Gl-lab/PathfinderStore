using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBackgroundPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedBackgroundFreeBoost",
                schema: "character_management",
                table: "Character",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedBackgroundId",
                schema: "character_management",
                table: "Character",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedBackgroundRestrictedBoost",
                schema: "character_management",
                table: "Character",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedBackgroundFreeBoost",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedBackgroundId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedBackgroundRestrictedBoost",
                schema: "character_management",
                table: "Character");
        }
    }
}
