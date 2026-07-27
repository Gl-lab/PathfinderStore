using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pathfinder.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachableRunes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachableRuneCode",
                schema: "inventory",
                table: "ItemInstance",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AttachedToInstanceKey",
                schema: "inventory",
                table: "ItemInstance",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RuneTargetKind",
                schema: "inventory",
                table: "ItemInstance",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemInstance_AttachedToInstanceKey",
                schema: "inventory",
                table: "ItemInstance",
                column: "AttachedToInstanceKey");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ItemInstance_Rune",
                schema: "inventory",
                table: "ItemInstance",
                sql: "(\"RuneTargetKind\" IS NULL AND \"AttachableRuneCode\" IS NULL AND \"AttachedToInstanceKey\" IS NULL) OR (\"RuneTargetKind\" IN (1, 2) AND ((\"AttachableRuneCode\" IS NULL AND \"AttachedToInstanceKey\" IS NULL) OR (\"AttachableRuneCode\" IS NOT NULL)))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ItemInstance_AttachedToInstanceKey",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ItemInstance_Rune",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "AttachableRuneCode",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "AttachedToInstanceKey",
                schema: "inventory",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "RuneTargetKind",
                schema: "inventory",
                table: "ItemInstance");
        }
    }
}
