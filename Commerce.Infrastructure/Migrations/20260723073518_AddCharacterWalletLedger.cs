using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterWalletLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wallet",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    BalanceCopper = table.Column<long>(type: "bigint", nullable: false),
                    ReservedCopper = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.Id);
                    table.CheckConstraint("CK_Wallet_Balance", "\"BalanceCopper\" >= 0 AND \"ReservedCopper\" >= 0 AND \"ReservedCopper\" <= \"BalanceCopper\"");
                    table.CheckConstraint("CK_Wallet_Identity", "\"CampaignId\" > 0 AND \"CharacterId\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "WalletLedgerEntry",
                schema: "commerce",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletId = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    AmountCopper = table.Column<long>(type: "bigint", nullable: false),
                    BalanceAfterCopper = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PerformedByUserId = table.Column<int>(type: "integer", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLedgerEntry", x => x.Id);
                    table.CheckConstraint("CK_WalletLedgerEntry_Actor", "\"PerformedByUserId\" > 0");
                    table.CheckConstraint("CK_WalletLedgerEntry_Amount", "\"AmountCopper\" <> 0");
                    table.ForeignKey(
                        name: "FK_WalletLedgerEntry_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "commerce",
                        principalTable: "Wallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CampaignId_CharacterId",
                schema: "commerce",
                table: "Wallet",
                columns: new[] { "CampaignId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletLedgerEntry_WalletId_OperationId",
                schema: "commerce",
                table: "WalletLedgerEntry",
                columns: new[] { "WalletId", "OperationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletLedgerEntry",
                schema: "commerce");

            migrationBuilder.DropTable(
                name: "Wallet",
                schema: "commerce");
        }
    }
}
