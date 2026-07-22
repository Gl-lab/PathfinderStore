using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryAuditEntry",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuditKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionKind = table.Column<int>(type: "integer", nullable: false),
                    ActorUserId = table.Column<int>(type: "integer", nullable: false),
                    IsForced = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ItemInstanceKey = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedKey = table.Column<Guid>(type: "uuid", nullable: true),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAuditEntry", x => x.Id);
                    table.CheckConstraint("CK_InventoryAuditEntry_Identity", "\"CampaignId\" > 0 AND \"ActorUserId\" > 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAuditEntry_AuditKey",
                schema: "inventory",
                table: "InventoryAuditEntry",
                column: "AuditKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAuditEntry_CampaignId_OperationId_ActionKind",
                schema: "inventory",
                table: "InventoryAuditEntry",
                columns: new[] { "CampaignId", "OperationId", "ActionKind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryAuditEntry",
                schema: "inventory");
        }
    }
}
