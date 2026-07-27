using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestockRunReviewLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublishedItemInstanceKey",
                schema: "commerce",
                table: "RestockRunLine",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PublishedOfferKey",
                schema: "commerce",
                table: "RestockRunLine",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                schema: "commerce",
                table: "RestockRun",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletedByUserId",
                schema: "commerce",
                table: "RestockRun",
                type: "integer",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RestockRunLine_Publication",
                schema: "commerce",
                table: "RestockRunLine",
                sql: "(\"PublishedOfferKey\" IS NULL AND \"PublishedItemInstanceKey\" IS NULL) OR (\"PublishedOfferKey\" IS NOT NULL AND ((\"Kind\" = 3 AND \"PublishedItemInstanceKey\" IS NOT NULL) OR (\"Kind\" <> 3 AND \"PublishedItemInstanceKey\" IS NULL)))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RestockRun_Lifecycle",
                schema: "commerce",
                table: "RestockRun",
                sql: "(\"Status\" = 1 AND \"CompletedByUserId\" IS NULL AND \"CompletedAtUtc\" IS NULL) OR (\"Status\" IN (2, 3) AND \"CompletedByUserId\" > 0 AND \"CompletedAtUtc\" IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RestockRunLine_Publication",
                schema: "commerce",
                table: "RestockRunLine");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RestockRun_Lifecycle",
                schema: "commerce",
                table: "RestockRun");

            migrationBuilder.DropColumn(
                name: "PublishedItemInstanceKey",
                schema: "commerce",
                table: "RestockRunLine");

            migrationBuilder.DropColumn(
                name: "PublishedOfferKey",
                schema: "commerce",
                table: "RestockRunLine");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                schema: "commerce",
                table: "RestockRun");

            migrationBuilder.DropColumn(
                name: "CompletedByUserId",
                schema: "commerce",
                table: "RestockRun");
        }
    }
}
