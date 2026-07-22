using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.ItemCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypedRuleComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AddColumn<int>(
                name: "PrimaryCategory",
                schema: "item_catalog",
                table: "ItemRevision",
                type: "integer",
                nullable: false,
                defaultValue: 9 );

            migrationBuilder.CreateTable(
                name: "ArmorComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    Category = table.Column<int>( type: "integer", nullable: false ),
                    ArmorClassBonus = table.Column<int>( type: "integer", nullable: false ),
                    DexterityCap = table.Column<int>( type: "integer", nullable: false ),
                    CheckPenalty = table.Column<int>( type: "integer", nullable: false ),
                    SpeedPenaltyFeet = table.Column<int>( type: "integer", nullable: false ),
                    StrengthRequirement = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ArmorComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_ArmorComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "AttackComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    Name = table.Column<string>( type: "character varying(100)", maxLength: 100, nullable: false ),
                    DamageDieCount = table.Column<int>( type: "integer", nullable: false ),
                    DamageDieSize = table.Column<int>( type: "integer", nullable: false ),
                    DamageType = table.Column<int>( type: "integer", nullable: false ),
                    Hands = table.Column<int>( type: "integer", nullable: false ),
                    RangeIncrementFeet = table.Column<int>( type: "integer", nullable: true )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_AttackComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_AttackComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "ChargeComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    MaximumCharges = table.Column<int>( type: "integer", nullable: false ),
                    DefaultActivationCost = table.Column<int>( type: "integer", nullable: false ),
                    RecoveryRule = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ChargeComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_ChargeComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "ConsumptionComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    Mode = table.Column<int>( type: "integer", nullable: false ),
                    Quantity = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ConsumptionComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_ConsumptionComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "DurabilityComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    Hardness = table.Column<int>( type: "integer", nullable: false ),
                    MaximumHitPoints = table.Column<int>( type: "integer", nullable: false ),
                    BrokenThreshold = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_DurabilityComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_DurabilityComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "EquipmentComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    Usage = table.Column<int>( type: "integer", nullable: false ),
                    RequiredHands = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_EquipmentComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_EquipmentComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateTable(
                name: "ShieldComponent",
                schema: "item_catalog",
                columns: table => new
                {
                    Id = table.Column<int>( type: "integer", nullable: false )
                        .Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
                    ItemRevisionId = table.Column<int>( type: "integer", nullable: false ),
                    RaisedArmorClassBonus = table.Column<int>( type: "integer", nullable: false )
                },
                constraints: table =>
                {
                    table.PrimaryKey( "PK_ShieldComponent", x => x.Id );
                    table.ForeignKey(
                        name: "FK_ShieldComponent_ItemRevision_ItemRevisionId",
                        column: x => x.ItemRevisionId,
                        principalSchema: "item_catalog",
                        principalTable: "ItemRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade );
                } );

            migrationBuilder.CreateIndex(
                name: "IX_ArmorComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "ArmorComponent",
                column: "ItemRevisionId",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_AttackComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "AttackComponent",
                column: "ItemRevisionId" );

            migrationBuilder.CreateIndex(
                name: "IX_ChargeComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "ChargeComponent",
                column: "ItemRevisionId",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_ConsumptionComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "ConsumptionComponent",
                column: "ItemRevisionId",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_DurabilityComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "DurabilityComponent",
                column: "ItemRevisionId",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "EquipmentComponent",
                column: "ItemRevisionId",
                unique: true );

            migrationBuilder.CreateIndex(
                name: "IX_ShieldComponent_ItemRevisionId",
                schema: "item_catalog",
                table: "ShieldComponent",
                column: "ItemRevisionId",
                unique: true );
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable(
                name: "ArmorComponent",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "AttackComponent",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "ChargeComponent",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "ConsumptionComponent",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "DurabilityComponent",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "EquipmentComponent",
                schema: "item_catalog" );

            migrationBuilder.DropTable(
                name: "ShieldComponent",
                schema: "item_catalog" );

            migrationBuilder.DropColumn(
                name: "PrimaryCategory",
                schema: "item_catalog",
                table: "ItemRevision" );
        }
    }
}