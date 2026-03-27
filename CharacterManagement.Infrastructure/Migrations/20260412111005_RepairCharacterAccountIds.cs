using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.CharacterManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RepairCharacterAccountIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE character_management."Character" AS character
                SET "AccountId" = account."Id"
                FROM character_management."Account" AS account
                WHERE ( character."AccountId" = account."UserId" )
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM character_management."Account" AS existingAccount
                      WHERE ( existingAccount."Id" = character."AccountId" )
                  );
                """ );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
