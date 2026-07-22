using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeFinalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CancelledAtUtc",
                schema: "inventory",
                table: "PartyExchange",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                schema: "inventory",
                table: "PartyExchange",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FinalOperationId",
                schema: "inventory",
                table: "PartyExchange",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "inventory",
                table: "PartyExchange",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                schema: "inventory",
                table: "PartyExchange");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                schema: "inventory",
                table: "PartyExchange");

            migrationBuilder.DropColumn(
                name: "FinalOperationId",
                schema: "inventory",
                table: "PartyExchange");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "inventory",
                table: "PartyExchange");
        }
    }
}
