using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClericSpellLoadout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreparedClericCantripIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "PreparedClericSpellIds",
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
                name: "PreparedClericCantripIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "PreparedClericSpellIds",
                schema: "character_management",
                table: "Character");
        }
    }
}
