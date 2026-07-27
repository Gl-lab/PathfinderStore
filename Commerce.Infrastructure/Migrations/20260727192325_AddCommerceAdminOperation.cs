using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommerceAdminOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommerceAdminOperation",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionKind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PayloadHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SettlementId = table.Column<int>(type: "integer", nullable: true),
                    ShopId = table.Column<int>(type: "integer", nullable: true),
                    OfferId = table.Column<int>(type: "integer", nullable: true),
                    PerformedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommerceAdminOperation", x => x.Id);
                    table.CheckConstraint("CK_CommerceAdminOperation_Identity", "\"CampaignId\" > 0 AND \"PerformedByUserId\" > 0");
                    table.ForeignKey(
                        name: "FK_CommerceAdminOperation_Settlement_SettlementId",
                        column: x => x.SettlementId,
                        principalSchema: "commerce",
                        principalTable: "Settlement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommerceAdminOperation_ShopOffer_OfferId",
                        column: x => x.OfferId,
                        principalSchema: "commerce",
                        principalTable: "ShopOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommerceAdminOperation_Shop_ShopId",
                        column: x => x.ShopId,
                        principalSchema: "commerce",
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommerceAdminOperation_CampaignId_OperationId",
                schema: "commerce",
                table: "CommerceAdminOperation",
                columns: new[] { "CampaignId", "OperationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommerceAdminOperation_OfferId",
                schema: "commerce",
                table: "CommerceAdminOperation",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceAdminOperation_SettlementId",
                schema: "commerce",
                table: "CommerceAdminOperation",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_CommerceAdminOperation_ShopId",
                schema: "commerce",
                table: "CommerceAdminOperation",
                column: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommerceAdminOperation",
                schema: "commerce");
        }
    }
}
