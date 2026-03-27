using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Account_1N_DraftCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Character_DraftCharacterId",
                schema: "character_management",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_DraftCharacterId",
                schema: "character_management",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "DraftCharacterId",
                schema: "character_management",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "character_management",
                table: "Character",
                newName: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_AccountId",
                schema: "character_management",
                table: "Character",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Character_Account_AccountId",
                schema: "character_management",
                table: "Character",
                column: "AccountId",
                principalSchema: "character_management",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Character_Account_AccountId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.DropIndex(
                name: "IX_Character_AccountId",
                schema: "character_management",
                table: "Character");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "character_management",
                table: "Character",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "DraftCharacterId",
                schema: "character_management",
                table: "Account",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_DraftCharacterId",
                schema: "character_management",
                table: "Account",
                column: "DraftCharacterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Character_DraftCharacterId",
                schema: "character_management",
                table: "Account",
                column: "DraftCharacterId",
                principalSchema: "character_management",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
