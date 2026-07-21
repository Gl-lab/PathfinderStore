using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterStartingEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedClassKitId",
                schema: "character_management",
                table: "Character",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedClassKitOptionIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "StartingEquipmentItems",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedClassKitId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedClassKitOptionIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "StartingEquipmentItems",
                schema: "character_management",
                table: "Character");
        }
    }
}
