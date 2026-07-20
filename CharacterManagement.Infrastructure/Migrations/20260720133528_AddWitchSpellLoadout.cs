using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWitchSpellLoadout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreparedWitchCantripIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "PreparedWitchSpellIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "SelectedWitchFocusHexId",
                schema: "character_management",
                table: "Character",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WitchFamiliarCantripIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "WitchFamiliarSpellIds",
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
                name: "PreparedWitchCantripIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "PreparedWitchSpellIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedWitchFocusHexId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "WitchFamiliarCantripIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "WitchFamiliarSpellIds",
                schema: "character_management",
                table: "Character");
        }
    }
}
