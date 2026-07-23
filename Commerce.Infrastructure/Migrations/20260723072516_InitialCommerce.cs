using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommerce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "commerce");

            migrationBuilder.CreateTable(
                name: "Settlement",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Region = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Traits = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlement", x => x.Id);
                    table.CheckConstraint("CK_Settlement_CampaignId", "\"CampaignId\" > 0");
                    table.CheckConstraint("CK_Settlement_Level", "(\"Level\" >= 0) AND (\"Level\" <= 20)");
                });

            migrationBuilder.CreateTable(
                name: "Shop",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SettlementId = table.Column<int>(type: "integer", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Specialization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShopLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop", x => x.Id);
                    table.CheckConstraint("CK_Shop_CampaignId", "\"CampaignId\" > 0");
                    table.CheckConstraint("CK_Shop_Level", "(\"ShopLevel\" >= 0) AND (\"ShopLevel\" <= 20)");
                    table.ForeignKey(
                        name: "FK_Shop_Settlement_SettlementId",
                        column: x => x.SettlementId,
                        principalSchema: "commerce",
                        principalTable: "Settlement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Settlement_CampaignId_Name",
                schema: "commerce",
                table: "Settlement",
                columns: new[] { "CampaignId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shop_CampaignId_Id",
                schema: "commerce",
                table: "Shop",
                columns: new[] { "CampaignId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Shop_SettlementId_Name",
                schema: "commerce",
                table: "Shop",
                columns: new[] { "SettlementId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shop",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "Settlement",
                schema: "commerce");
        }
    }
}
