using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pathfinder.Infrastructure.Migrations
{
    public partial class RenameArticlesToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalDamage_DamageTypeList_DamageTypeId",
                table: "AdditionalDamage");

            migrationBuilder.DropForeignKey(
                name: "FK_BackpackItem_Items_ItemId",
                table: "BackpackItem");

            migrationBuilder.DropForeignKey(
                name: "FK_DamageTypeList_WeaponList_WeaponId",
                table: "DamageTypeList");

            migrationBuilder.DropForeignKey(
                name: "FK_Effect_ArticleList_ArticleId",
                table: "Effect");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ArticleList_ArticleId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopsProducts_Items_ItemId",
                table: "ShopsProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponItemProperty_Items_ItemId",
                table: "WeaponItemProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponList_Ammunition_AmmunitionId",
                table: "WeaponList");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponList_ArticleList_ArticleId",
                table: "WeaponList");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponList_Dices_MediumSizeDamageId",
                table: "WeaponList");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponList_Dices_SmallSizeDamageId",
                table: "WeaponList");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponList_WeaponTypeList_WeaponTypeId",
                table: "WeaponList");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponTypeList_WeaponProficiency_WeaponProficiencyId",
                table: "WeaponTypeList");

            migrationBuilder.DropTable(
                name: "ArticleList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeaponTypeList",
                table: "WeaponTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeaponList",
                table: "WeaponList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DamageTypeList",
                table: "DamageTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CharacteristicInfo",
                table: "CharacteristicInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryList",
                table: "CategoryList");

            migrationBuilder.RenameTable(
                name: "WeaponTypeList",
                newName: "WeaponType");

            migrationBuilder.RenameTable(
                name: "WeaponList",
                newName: "Weapon");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Item");

            migrationBuilder.RenameTable(
                name: "DamageTypeList",
                newName: "DamageType");

            migrationBuilder.RenameTable(
                name: "CharacteristicInfo",
                newName: "CharacteristicType");

            migrationBuilder.RenameTable(
                name: "CategoryList",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "Effect",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Effect_ArticleId",
                table: "Effect",
                newName: "IX_Effect_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponTypeList_WeaponProficiencyId",
                table: "WeaponType",
                newName: "IX_WeaponType_WeaponProficiencyId");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "Weapon",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponList_WeaponTypeId",
                table: "Weapon",
                newName: "IX_Weapon_WeaponTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponList_SmallSizeDamageId",
                table: "Weapon",
                newName: "IX_Weapon_SmallSizeDamageId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponList_MediumSizeDamageId",
                table: "Weapon",
                newName: "IX_Weapon_MediumSizeDamageId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponList_ArticleId",
                table: "Weapon",
                newName: "IX_Weapon_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponList_AmmunitionId",
                table: "Weapon",
                newName: "IX_Weapon_AmmunitionId");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "Item",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_ArticleId",
                table: "Item",
                newName: "IX_Item_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_DamageTypeList_WeaponId",
                table: "DamageType",
                newName: "IX_DamageType_WeaponId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModificationDate",
                table: "Permission",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Permission",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeaponType",
                table: "WeaponType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weapon",
                table: "Weapon",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Item",
                table: "Item",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DamageType",
                table: "DamageType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CharacteristicType",
                table: "CharacteristicType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "CategoryType");

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageFile = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    CategoryType = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryType",
                        column: x => x.CategoryType,
                        principalTable: "Category",
                        principalColumn: "CategoryType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "292963a7-1ca0-4f1c-9acc-dc8cd976602c");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "a1b665fc-8fa8-423d-8906-06eb178373f5");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "ccf84999-84d1-4365-ab11-9d9c34710b41");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "9747ceed-0095-40ab-953c-1b98459fd7a0");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "b698fa94-e6a5-4cc8-b7c3-397aa4bee9e5");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryType",
                table: "Product",
                column: "CategoryType");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalDamage_DamageType_DamageTypeId",
                table: "AdditionalDamage",
                column: "DamageTypeId",
                principalTable: "DamageType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BackpackItem_Item_ItemId",
                table: "BackpackItem",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageType_Weapon_WeaponId",
                table: "DamageType",
                column: "WeaponId",
                principalTable: "Weapon",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Effect_Product_ProductId",
                table: "Effect",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Product_ProductId",
                table: "Item",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsProducts_Item_ItemId",
                table: "ShopsProducts",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapon_Ammunition_AmmunitionId",
                table: "Weapon",
                column: "AmmunitionId",
                principalTable: "Ammunition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapon_Dices_MediumSizeDamageId",
                table: "Weapon",
                column: "MediumSizeDamageId",
                principalTable: "Dices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapon_Dices_SmallSizeDamageId",
                table: "Weapon",
                column: "SmallSizeDamageId",
                principalTable: "Dices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapon_Product_ProductId",
                table: "Weapon",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weapon_WeaponType_WeaponTypeId",
                table: "Weapon",
                column: "WeaponTypeId",
                principalTable: "WeaponType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponItemProperty_Item_ItemId",
                table: "WeaponItemProperty",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponType_WeaponProficiency_WeaponProficiencyId",
                table: "WeaponType",
                column: "WeaponProficiencyId",
                principalTable: "WeaponProficiency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalDamage_DamageType_DamageTypeId",
                table: "AdditionalDamage");

            migrationBuilder.DropForeignKey(
                name: "FK_BackpackItem_Item_ItemId",
                table: "BackpackItem");

            migrationBuilder.DropForeignKey(
                name: "FK_DamageType_Weapon_WeaponId",
                table: "DamageType");

            migrationBuilder.DropForeignKey(
                name: "FK_Effect_Product_ProductId",
                table: "Effect");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_Product_ProductId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopsProducts_Item_ItemId",
                table: "ShopsProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapon_Ammunition_AmmunitionId",
                table: "Weapon");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapon_Dices_MediumSizeDamageId",
                table: "Weapon");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapon_Dices_SmallSizeDamageId",
                table: "Weapon");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapon_Product_ProductId",
                table: "Weapon");

            migrationBuilder.DropForeignKey(
                name: "FK_Weapon_WeaponType_WeaponTypeId",
                table: "Weapon");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponItemProperty_Item_ItemId",
                table: "WeaponItemProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_WeaponType_WeaponProficiency_WeaponProficiencyId",
                table: "WeaponType");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeaponType",
                table: "WeaponType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weapon",
                table: "Weapon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Item",
                table: "Item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DamageType",
                table: "DamageType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CharacteristicType",
                table: "CharacteristicType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "WeaponType",
                newName: "WeaponTypeList");

            migrationBuilder.RenameTable(
                name: "Weapon",
                newName: "WeaponList");

            migrationBuilder.RenameTable(
                name: "Item",
                newName: "Items");

            migrationBuilder.RenameTable(
                name: "DamageType",
                newName: "DamageTypeList");

            migrationBuilder.RenameTable(
                name: "CharacteristicType",
                newName: "CharacteristicInfo");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "CategoryList");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Effect",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Effect_ProductId",
                table: "Effect",
                newName: "IX_Effect_ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_WeaponType_WeaponProficiencyId",
                table: "WeaponTypeList",
                newName: "IX_WeaponTypeList_WeaponProficiencyId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "WeaponList",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_WeaponTypeId",
                table: "WeaponList",
                newName: "IX_WeaponList_WeaponTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_SmallSizeDamageId",
                table: "WeaponList",
                newName: "IX_WeaponList_SmallSizeDamageId");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_ProductId",
                table: "WeaponList",
                newName: "IX_WeaponList_ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_MediumSizeDamageId",
                table: "WeaponList",
                newName: "IX_WeaponList_MediumSizeDamageId");

            migrationBuilder.RenameIndex(
                name: "IX_Weapon_AmmunitionId",
                table: "WeaponList",
                newName: "IX_WeaponList_AmmunitionId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Items",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Item_ProductId",
                table: "Items",
                newName: "IX_Items_ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_DamageType_WeaponId",
                table: "DamageTypeList",
                newName: "IX_DamageTypeList_WeaponId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModificationDate",
                table: "Permission",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Permission",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeaponTypeList",
                table: "WeaponTypeList",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeaponList",
                table: "WeaponList",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DamageTypeList",
                table: "DamageTypeList",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CharacteristicInfo",
                table: "CharacteristicInfo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryList",
                table: "CategoryList",
                column: "CategoryType");

            migrationBuilder.CreateTable(
                name: "ArticleList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryType = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageFile = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleList_CategoryList_CategoryType",
                        column: x => x.CategoryType,
                        principalTable: "CategoryList",
                        principalColumn: "CategoryType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "eeb43634-02f1-49ca-8127-30dc3e8e96e3");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "76105df9-5c5f-4cd4-a451-38f565722f17");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "ad5ed443-af06-4fef-afa7-0a3ceb8b9e72");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "acff2463-0ae9-44fe-aea7-f4d0fc24913a");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "9eaf0ded-3e93-4c42-ba36-0cd0becb0a92");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleList_CategoryType",
                table: "ArticleList",
                column: "CategoryType");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalDamage_DamageTypeList_DamageTypeId",
                table: "AdditionalDamage",
                column: "DamageTypeId",
                principalTable: "DamageTypeList",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BackpackItem_Items_ItemId",
                table: "BackpackItem",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageTypeList_WeaponList_WeaponId",
                table: "DamageTypeList",
                column: "WeaponId",
                principalTable: "WeaponList",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Effect_ArticleList_ArticleId",
                table: "Effect",
                column: "ArticleId",
                principalTable: "ArticleList",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ArticleList_ArticleId",
                table: "Items",
                column: "ArticleId",
                principalTable: "ArticleList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsProducts_Items_ItemId",
                table: "ShopsProducts",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponItemProperty_Items_ItemId",
                table: "WeaponItemProperty",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponList_Ammunition_AmmunitionId",
                table: "WeaponList",
                column: "AmmunitionId",
                principalTable: "Ammunition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponList_ArticleList_ArticleId",
                table: "WeaponList",
                column: "ArticleId",
                principalTable: "ArticleList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponList_Dices_MediumSizeDamageId",
                table: "WeaponList",
                column: "MediumSizeDamageId",
                principalTable: "Dices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponList_Dices_SmallSizeDamageId",
                table: "WeaponList",
                column: "SmallSizeDamageId",
                principalTable: "Dices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponList_WeaponTypeList_WeaponTypeId",
                table: "WeaponList",
                column: "WeaponTypeId",
                principalTable: "WeaponTypeList",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponTypeList_WeaponProficiency_WeaponProficiencyId",
                table: "WeaponTypeList",
                column: "WeaponProficiencyId",
                principalTable: "WeaponProficiency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
