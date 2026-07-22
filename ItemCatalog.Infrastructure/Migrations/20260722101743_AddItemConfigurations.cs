using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.CreateTable(
                name: "ItemConfiguration",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    ConfigurationKey = table.Column<string>( type: "character(64)", fixedLength: true, maxLength: 64, nullable: false ),
                    Size = table.Column<int>( type: "integer", nullable: false ),
                    MaterialType = table.Column<int>( type: "integer", nullable: false ),
                    MaterialGrade = table.Column<int>( type: "integer", nullable: false ),
                    CreatedAtUtc = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ItemConfiguration", x => x.Id );
                    table.ForeignKey(
                        name: "FK_ItemConfiguration_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict );
                } );

            migrationBuilder.CreateTable(
                name: "PermanentUpgrade",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemConfigurationId = table.Column<int>( type: "integer", nullable: false ),
                    Code = table.Column<string>( type: "character varying(100)", maxLength: 100, nullable: false ),
                    Kind = table.Column<int>( type: "integer", nullable: false ),
                    Rank = table.Column<int>( type: "integer", nullable: false ),
                    Visibility = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_PermanentUpgrade", x => x.Id );
                    table.ForeignKey(
                        name: "FK_PermanentUpgrade_ItemConfiguration_ItemConfigurationId",
                        column: x => x.ItemConfigurationId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateIndex(
                name: "IX_ItemConfiguration_ConfigurationKey",
                schema: "item_catalog",
                table: "ItemConfiguration",
                column: "ConfigurationKey",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_ItemConfiguration_ItemRevisionId",
                schema: "item_catalog",
                table: "ItemConfiguration",
                column: "ItemRevisionId" );

            migrationBuilder.CreateIndex(
                name: "IX_PermanentUpgrade_ItemConfigurationId_Code",
                schema: "item_catalog",
                table: "PermanentUpgrade",
                columns: new[] { "ItemConfigurationId", "Code" },
                unique: true );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "PermanentUpgrade",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "ItemConfiguration",
                schema: "item_catalog" );
        }
    }
}