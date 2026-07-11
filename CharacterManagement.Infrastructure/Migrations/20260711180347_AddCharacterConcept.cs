using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterConcept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Concept",
                schema: "character_management",
                table: "Character",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Concept",
                schema: "character_management",
                table: "Character");
        }
    }
}
