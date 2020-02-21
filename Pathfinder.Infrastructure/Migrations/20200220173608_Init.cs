using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pathfinder.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ammunition",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ammunition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiceList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    D = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiceList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponTypeList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    ShortNAme = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponTypeList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ImageFile = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductList_CategoryList_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(nullable: false),
                    CountDiceDamage = table.Column<int>(nullable: false),
                    SizeDiceDamageId = table.Column<int>(nullable: true),
                    Range = table.Column<int>(nullable: true),
                    AmmunitionId = table.Column<int>(nullable: true),
                    MultiplierCrit = table.Column<int>(nullable: false),
                    CritRange = table.Column<int>(nullable: false),
                    WeaponTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponList_Ammunition_AmmunitionId",
                        column: x => x.AmmunitionId,
                        principalTable: "Ammunition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeaponList_ProductList_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponList_DiceList_SizeDiceDamageId",
                        column: x => x.SizeDiceDamageId,
                        principalTable: "DiceList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeaponList_WeaponTypeList_WeaponTypeId",
                        column: x => x.WeaponTypeId,
                        principalTable: "WeaponTypeList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageTypeList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    WeaponId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageTypeList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageTypeList_WeaponList_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "WeaponList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DamageTypeList_WeaponId",
                table: "DamageTypeList",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductList_CategoryId",
                table: "ProductList",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_AmmunitionId",
                table: "WeaponList",
                column: "AmmunitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_ProductId",
                table: "WeaponList",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_SizeDiceDamageId",
                table: "WeaponList",
                column: "SizeDiceDamageId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_WeaponTypeId",
                table: "WeaponList",
                column: "WeaponTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DamageTypeList");

            migrationBuilder.DropTable(
                name: "WeaponList");

            migrationBuilder.DropTable(
                name: "Ammunition");

            migrationBuilder.DropTable(
                name: "ProductList");

            migrationBuilder.DropTable(
                name: "DiceList");

            migrationBuilder.DropTable(
                name: "WeaponTypeList");

            migrationBuilder.DropTable(
                name: "CategoryList");
        }
    }
}
