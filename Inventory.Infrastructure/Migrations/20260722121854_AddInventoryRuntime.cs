using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryRuntime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.CreateTable(
                name: "InventoryContainer",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContainerKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    OwnerKind = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryContainer", x => x.Id);
                    table.UniqueConstraint("AK_InventoryContainer_CampaignId_ContainerKey", x => new { x.CampaignId, x.ContainerKey });
                    table.CheckConstraint("CK_InventoryContainer_PositiveIds", "\"CampaignId\" > 0 AND \"OwnerId\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "ItemInstance",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstanceKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    ItemConfigurationId = table.Column<int>(type: "integer", nullable: false),
                    CustomName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsStackable = table.Column<bool>(type: "boolean", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CurrentContainerKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemInstance", x => x.Id);
                    table.CheckConstraint("CK_ItemInstance_State", "\"CampaignId\" > 0 AND \"ItemConfigurationId\" > 0 AND \"Quantity\" >= 0");
                    table.ForeignKey(
                        name: "FK_ItemInstance_InventoryContainer_CampaignId_CurrentContainer~",
                        columns: x => new { x.CampaignId, x.CurrentContainerKey },
                        principalSchema: "inventory",
                        principalTable: "InventoryContainer",
                        principalColumns: new[] { "CampaignId", "ContainerKey" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovement",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemInstanceKey = table.Column<Guid>(type: "uuid", nullable: false),
                    FromContainerKey = table.Column<Guid>(type: "uuid", nullable: false),
                    ToContainerKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemInstanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovement", x => x.Id);
                    table.CheckConstraint("CK_InventoryMovement_Quantity", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_InventoryMovement_ItemInstance_ItemInstanceId",
                        column: x => x.ItemInstanceId,
                        principalSchema: "inventory",
                        principalTable: "ItemInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryOperation",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    RelatedKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    VersionAfter = table.Column<int>(type: "integer", nullable: false),
                    AppliedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemInstanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryOperation", x => x.Id);
                    table.CheckConstraint("CK_InventoryOperation_State", "\"Quantity\" > 0 AND \"VersionAfter\" > 0");
                    table.ForeignKey(
                        name: "FK_InventoryOperation_ItemInstance_ItemInstanceId",
                        column: x => x.ItemInstanceId,
                        principalSchema: "inventory",
                        principalTable: "ItemInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryContainer_CampaignId_ContainerKey",
                schema: "inventory",
                table: "InventoryContainer",
                columns: new[] { "CampaignId", "ContainerKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryContainer_CampaignId_OwnerKind_OwnerId",
                schema: "inventory",
                table: "InventoryContainer",
                columns: new[] { "CampaignId", "OwnerKind", "OwnerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryContainer_ContainerKey",
                schema: "inventory",
                table: "InventoryContainer",
                column: "ContainerKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_ItemInstanceId_OperationId",
                schema: "inventory",
                table: "InventoryMovement",
                columns: new[] { "ItemInstanceId", "OperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperation_ItemInstanceId_OperationId",
                schema: "inventory",
                table: "InventoryOperation",
                columns: new[] { "ItemInstanceId", "OperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemInstance_CampaignId_CurrentContainerKey",
                schema: "inventory",
                table: "ItemInstance",
                columns: new[] { "CampaignId", "CurrentContainerKey" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemInstance_InstanceKey",
                schema: "inventory",
                table: "ItemInstance",
                column: "InstanceKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemInstance_ItemConfigurationId",
                schema: "inventory",
                table: "ItemInstance",
                column: "ItemConfigurationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryMovement",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "InventoryOperation",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "ItemInstance",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "InventoryContainer",
                schema: "inventory");
        }
    }
}
