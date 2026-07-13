using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedClassId",
                schema: "character_management",
                table: "Character",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedClassKeyAbility",
                schema: "character_management",
                table: "Character",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedClassId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedClassKeyAbility",
                schema: "character_management",
                table: "Character");
        }
    }
}
