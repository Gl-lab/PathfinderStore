using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRaceAncestryOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Character_Race_RaceId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropTable(
                name: "Race",
                schema: "character_management");

            migrationBuilder.DropIndex(
                name: "IX_Character_RaceId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropColumn(
                name: "RaceId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.AlterColumn<int>(
                name: "AncestryType",
                schema: "character_management",
                table: "Character",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AncestryType",
                schema: "character_management",
                table: "Character",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "RaceId",
                schema: "character_management",
                table: "Character",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Race",
                schema: "character_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseSpeed = table.Column<int>(type: "integer", nullable: false),
                    IsNightVision = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Race", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Character_RaceId",
                schema: "character_management",
                table: "Character",
                column: "RaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Character_Race_RaceId",
                schema: "character_management",
                table: "Character",
                column: "RaceId",
                principalSchema: "character_management",
                principalTable: "Race",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
