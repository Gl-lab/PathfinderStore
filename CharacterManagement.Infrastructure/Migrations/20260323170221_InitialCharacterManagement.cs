using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCharacterManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "character_management");

            migrationBuilder.CreateTable(
                name: "Race",
                schema: "character_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeId = table.Column<int>(type: "integer", nullable: false),
                    BaseSpeed = table.Column<int>(type: "integer", nullable: false),
                    IsNightVision = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Race", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Character",
                schema: "character_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RaceId = table.Column<int>(type: "integer", nullable: false),
                    AncestryType = table.Column<int>(type: "integer", nullable: true),
                    Str = table.Column<int>(type: "integer", nullable: false),
                    Dex = table.Column<int>(type: "integer", nullable: false),
                    Con = table.Column<int>(type: "integer", nullable: false),
                    Int = table.Column<int>(type: "integer", nullable: false),
                    Wis = table.Column<int>(type: "integer", nullable: false),
                    Cha = table.Column<int>(type: "integer", nullable: false),
                    AppliedFreeBoosts = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Race_RaceId",
                        column: x => x.RaceId,
                        principalSchema: "character_management",
                        principalTable: "Race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "character_management",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DraftCharacterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Character_DraftCharacterId",
                        column: x => x.DraftCharacterId,
                        principalSchema: "character_management",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_DraftCharacterId",
                schema: "character_management",
                table: "Account",
                column: "DraftCharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Character_RaceId",
                schema: "character_management",
                table: "Character",
                column: "RaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account",
                schema: "character_management");

            migrationBuilder.DropTable(
                name: "Character",
                schema: "character_management");

            migrationBuilder.DropTable(
                name: "Race",
                schema: "character_management");
        }
    }
}
