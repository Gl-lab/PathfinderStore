using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReproducibleRestockRuns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestockRun",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RunKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    RestockPolicyId = table.Column<int>(type: "integer", nullable: false),
                    PolicyVersion = table.Column<int>(type: "integer", nullable: false),
                    Seed = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestockRun", x => x.Id);
                    table.CheckConstraint("CK_RestockRun_Identity", "\"CampaignId\" > 0 AND \"ShopId\" > 0 AND \"RestockPolicyId\" > 0 AND \"PolicyVersion\" > 0 AND \"CreatedByUserId\" > 0");
                    table.ForeignKey(
                        name: "FK_RestockRun_RestockPolicy_RestockPolicyId",
                        column: x => x.RestockPolicyId,
                        principalSchema: "commerce",
                        principalTable: "RestockPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestockRun_Shop_ShopId",
                        column: x => x.ShopId,
                        principalSchema: "commerce",
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestockRunLine",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestockRunId = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    ItemConfigurationId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceCopper = table.Column<long>(type: "bigint", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestockRunLine", x => x.Id);
                    table.CheckConstraint("CK_RestockRunLine_Values", "\"Sequence\" > 0 AND \"ItemConfigurationId\" > 0 AND \"Quantity\" > 0 AND \"UnitPriceCopper\" >= 0");
                    table.ForeignKey(
                        name: "FK_RestockRunLine_RestockRun_RestockRunId",
                        column: x => x.RestockRunId,
                        principalSchema: "commerce",
                        principalTable: "RestockRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestockRun_RestockPolicyId",
                schema: "commerce",
                table: "RestockRun",
                column: "RestockPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_RestockRun_RunKey",
                schema: "commerce",
                table: "RestockRun",
                column: "RunKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestockRun_ShopId_RestockPolicyId_PolicyVersion_Seed",
                schema: "commerce",
                table: "RestockRun",
                columns: new[] { "ShopId", "RestockPolicyId", "PolicyVersion", "Seed" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestockRunLine_RestockRunId_ItemConfigurationId",
                schema: "commerce",
                table: "RestockRunLine",
                columns: new[] { "RestockRunId", "ItemConfigurationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestockRunLine_RestockRunId_Sequence",
                schema: "commerce",
                table: "RestockRunLine",
                columns: new[] { "RestockRunId", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestockRunLine",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "RestockRun",
                schema: "commerce");
        }
    }
}
