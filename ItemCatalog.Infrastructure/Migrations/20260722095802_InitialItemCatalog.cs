using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialItemCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.EnsureSchema(
                name: "item_catalog" );

            migrationBuilder.CreateTable(
                name: "ItemDefinition",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    Key = table.Column<string>( type: "character varying(200)", maxLength: 200, nullable: false ),
                    CreatedAtUtc = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ItemDefinition", x => x.Id );
                } );

            migrationBuilder.CreateTable(
                name: "ItemRevision",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemDefinitionId = table.Column<int>( type: "integer", nullable: false ),
                    RevisionNumber = table.Column<int>( type: "integer", nullable: false ),
                    Name = table.Column<string>( type: "character varying(200)", maxLength: 200, nullable: false ),
                    Description = table.Column<string>( type: "character varying(4000)", maxLength: 4000, nullable: false ),
                    Level = table.Column<int>( type: "integer", nullable: false ),
                    PriceInCopperPieces = table.Column<int>( type: "integer", nullable: false ),
                    Bulk = table.Column<decimal>( type: "numeric(8,2)", precision: 8, scale: 2, nullable: false ),
                    CreatedAtUtc = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ItemRevision", x => x.Id );
                    table.ForeignKey(
                        name: "FK_ItemRevision_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_Key",
                schema: "item_catalog",
                table: "ItemDefinition",
                column: "Key",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_ItemRevision_ItemDefinitionId_RevisionNumber",
                schema: "item_catalog",
                table: "ItemRevision",
                columns: new[] { "ItemDefinitionId", "RevisionNumber" },
                unique: true );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "ItemRevision",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "ItemDefinition",
                schema: "item_catalog" );
        }
    }
}