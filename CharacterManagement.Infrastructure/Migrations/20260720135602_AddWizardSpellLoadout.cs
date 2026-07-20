using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWizardSpellLoadout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreparedWizardCantripIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "PreparedWizardSpellIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "SelectedPreparedWizardCurriculumCantripId",
                schema: "character_management",
                table: "Character",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedPreparedWizardCurriculumSpellId",
                schema: "character_management",
                table: "Character",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedWizardCurriculumCantripId",
                schema: "character_management",
                table: "Character",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WizardCurriculumSpellIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "WizardSpellbookCantripIds",
                schema: "character_management",
                table: "Character",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "WizardSpellbookSpellIds",
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
                name: "PreparedWizardCantripIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "PreparedWizardSpellIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedPreparedWizardCurriculumCantripId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedPreparedWizardCurriculumSpellId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "SelectedWizardCurriculumCantripId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "WizardCurriculumSpellIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "WizardSpellbookCantripIds",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "WizardSpellbookSpellIds",
                schema: "character_management",
                table: "Character");
        }
    }
}
