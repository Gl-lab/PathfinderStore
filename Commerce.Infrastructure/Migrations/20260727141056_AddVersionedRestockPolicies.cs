using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionedRestockPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestockPolicy",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestockPolicy", x => x.Id);
                    table.CheckConstraint("CK_RestockPolicy_Identity", "\"CampaignId\" > 0 AND \"ShopId\" > 0 AND \"CurrentVersion\" > 0");
                    table.ForeignKey(
                        name: "FK_RestockPolicy_Shop_ShopId",
                        column: x => x.ShopId,
                        principalSchema: "commerce",
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestockPolicyRevision",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestockPolicyId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    TargetOfferCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestockPolicyRevision", x => x.Id);
                    table.CheckConstraint("CK_RestockPolicyRevision_Values", "\"Version\" > 0 AND \"TargetOfferCount\" > 0 AND \"CreatedByUserId\" > 0");
                    table.ForeignKey(
                        name: "FK_RestockPolicyRevision_RestockPolicy_RestockPolicyId",
                        column: x => x.RestockPolicyId,
                        principalSchema: "commerce",
                        principalTable: "RestockPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestockPolicy_ShopId",
                schema: "commerce",
                table: "RestockPolicy",
                column: "ShopId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestockPolicyRevision_RestockPolicyId_Version",
                schema: "commerce",
                table: "RestockPolicyRevision",
                columns: new[] { "RestockPolicyId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestockPolicyRevision",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "RestockPolicy",
                schema: "commerce");
        }
    }
}
