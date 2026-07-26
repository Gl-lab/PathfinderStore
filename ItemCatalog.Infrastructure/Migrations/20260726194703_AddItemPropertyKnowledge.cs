using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemPropertyKnowledge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemPropertyKnowledge",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    InstanceKey = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectKind = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    UpgradeCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RevealedByUserId = table.Column<int>(type: "integer", nullable: false),
                    RevealedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPropertyKnowledge", x => x.Id);
                    table.CheckConstraint("CK_ItemPropertyKnowledge_Identity", "\"CampaignId\" > 0 AND \"SubjectId\" > 0 AND \"RevealedByUserId\" > 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPropertyKnowledge_CampaignId_InstanceKey_SubjectKind_Su~",
                schema: "item_catalog",
                table: "ItemPropertyKnowledge",
                columns: new[] { "CampaignId", "InstanceKey", "SubjectKind", "SubjectId", "UpgradeCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemPropertyKnowledge",
                schema: "item_catalog");
        }
    }
}
