using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pathfinder.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ammunition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ammunition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseDice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    D = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseDice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceSize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceSize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSystemDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponTypeList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponTypeList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    DId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dices_BaseDice_DId",
                        column: x => x.DId,
                        principalTable: "BaseDice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageFile = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleList_CategoryList_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characteristic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacteristicInfoId = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characteristic_CharacteristicInfo_CharacteristicInfoId",
                        column: x => x.CharacteristicInfoId,
                        principalTable: "CharacteristicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Race",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SizeId = table.Column<int>(type: "integer", nullable: true),
                    BaseSpeed = table.Column<int>(type: "integer", nullable: false),
                    HaveNigthVision = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Race", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Race_RaceSize_SizeId",
                        column: x => x.SizeId,
                        principalTable: "RaceSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Effect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ArticleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Effect_ArticleList_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ArticleList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArticleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ArticleList_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ArticleList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeaponList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArticleId = table.Column<int>(type: "integer", nullable: false),
                    Range = table.Column<int>(type: "integer", nullable: true),
                    MultiplierCrit = table.Column<int>(type: "integer", nullable: false),
                    CritRange = table.Column<int>(type: "integer", nullable: false),
                    AmmunitionId = table.Column<int>(type: "integer", nullable: true),
                    WeaponTypeId = table.Column<int>(type: "integer", nullable: true),
                    SmallSizeDamageId = table.Column<int>(type: "integer", nullable: true),
                    MediumSizeDamageId = table.Column<int>(type: "integer", nullable: true),
                    LargeSizeDamageId = table.Column<int>(type: "integer", nullable: true)
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
                        name: "FK_WeaponList_ArticleList_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "ArticleList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponList_Dices_LargeSizeDamageId",
                        column: x => x.LargeSizeDamageId,
                        principalTable: "Dices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeaponList_Dices_MediumSizeDamageId",
                        column: x => x.MediumSizeDamageId,
                        principalTable: "Dices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeaponList_Dices_SmallSizeDamageId",
                        column: x => x.SmallSizeDamageId,
                        principalTable: "Dices",
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
                name: "GroupCharacteristic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StrengthId = table.Column<int>(type: "integer", nullable: true),
                    DexterityId = table.Column<int>(type: "integer", nullable: true),
                    ConstitutionId = table.Column<int>(type: "integer", nullable: true),
                    IntelligenceId = table.Column<int>(type: "integer", nullable: true),
                    WisdomId = table.Column<int>(type: "integer", nullable: true),
                    CharismaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupCharacteristic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupCharacteristic_Characteristic_CharismaId",
                        column: x => x.CharismaId,
                        principalTable: "Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupCharacteristic_Characteristic_ConstitutionId",
                        column: x => x.ConstitutionId,
                        principalTable: "Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupCharacteristic_Characteristic_DexterityId",
                        column: x => x.DexterityId,
                        principalTable: "Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupCharacteristic_Characteristic_IntelligenceId",
                        column: x => x.IntelligenceId,
                        principalTable: "Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupCharacteristic_Characteristic_StrengthId",
                        column: x => x.StrengthId,
                        principalTable: "Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupCharacteristic_Characteristic_WisdomId",
                        column: x => x.WisdomId,
                        principalTable: "Characteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopsProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShopId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopsProducts_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopsProducts_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeaponItemProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeaponId = table.Column<int>(type: "integer", nullable: true),
                    IsMasterful = table.Column<bool>(type: "boolean", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponItemProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponItemProperty_Items_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageTypeList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    WeaponId = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Balance = table.Column<int>(type: "integer", nullable: false),
                    RaceId = table.Column<int>(type: "integer", nullable: true),
                    CharacteristicsId = table.Column<int>(type: "integer", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Character_GroupCharacteristic_CharacteristicsId",
                        column: x => x.CharacteristicsId,
                        principalTable: "GroupCharacteristic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Character_Race_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Backpack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    IsWearable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Backpack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Backpack_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Backpack_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreationDate", "CreatorId", "DisplayName", "ModificationDate", "Name" },
                values: new object[,]
                {
                    { new Guid("2a1ccb43-fa4f-48ce-b601-d3ab4d611b32"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Administration access", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_Administration" },
                    { new Guid("28126ffd-51c2-4201-939c-b64e3df43b9d"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Member access", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_Member_Access" },
                    { new Guid("86d804bd-d022-49a5-821a-d2240478aac4"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "User read", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_User_Read" },
                    { new Guid("8f3de3ec-3851-4ba9-887a-2119f18ae744"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "User create", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_User_Create" },
                    { new Guid("068a0171-a141-4eb2-854c-88e43ef9ab7f"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "User update", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_User_Update" },
                    { new Guid("70b5c5c3-2267-4f7c-b0f9-7ecc952e04a6"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "User delete", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_User_Delete" },
                    { new Guid("80562f50-8a7d-4bcd-8971-6d856bbcbb7f"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Role read", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_Role_Read" },
                    { new Guid("d4d7c0e3-efcf-4dd2-86e7-17d69fda8c75"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Role create", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_Role_Create" },
                    { new Guid("ea003a99-4755-4c19-b126-c5cffbc65088"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Role update", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_Role_Update" },
                    { new Guid("8f76de0b-114a-4df8-a93d-cec927d06a3c"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), "Role delete", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permissions_Role_Delete" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "IsSystemDefault", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263"), "d06f9c83-c5be-4476-9312-328107c92222", true, "Admin", "ADMIN" },
                    { new Guid("11d14a89-3a93-4d39-a94f-82b823f0d4ce"), "e81fb60f-b088-4558-89b0-0d1417ddf574", true, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("c41a7761-6645-4e2c-b99d-f9e767b9ac77"), 5, "9aaf4656-e943-4b0a-bddf-f0eeea314894", "admin@mail.com", true, false, null, "ADMIN@MAIL.COM", "ADMIN", "AQAAAAEAACcQAAAAEPZ85aLX3oCrOfmyD5aQWLzR2F53RxX0ko4ManWsiPXnbeKcL3YMOIKANLWsukGMzw==", null, false, null, false, "admin" },
                    { new Guid("065e903e-6f7b-42b8-b807-0c4197f9d1bc"), 5, "e4bc690e-195a-44bf-93a6-b8619777e084", "memberuser@mail.com", true, false, null, "MEMBERUSER@MAIL.COM", "MEMBERUSER", "AQAAAAEAACcQAAAAEPZ85aLX3oCrOfmyD5aQWLzR2F53RxX0ko4ManWsiPXnbeKcL3YMOIKANLWsukGMzw==", null, false, null, false, "memberuser" },
                    { new Guid("4b6d9e45-626d-489a-a8cf-d32d36583af4"), 5, "01000ec8-3510-44a1-9196-1583b43e9b62", "testadmin@mail.com", true, false, null, "TESTADMIN@MAIL.COM", "TESTADMIN", "AQAAAAEAACcQAAAAEPZ85aLX3oCrOfmyD5aQWLzR2F53RxX0ko4ManWsiPXnbeKcL3YMOIKANLWsukGMzw==", null, false, null, false, "testadmin" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("2a1ccb43-fa4f-48ce-b601-d3ab4d611b32"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("28126ffd-51c2-4201-939c-b64e3df43b9d"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("86d804bd-d022-49a5-821a-d2240478aac4"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("8f3de3ec-3851-4ba9-887a-2119f18ae744"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("068a0171-a141-4eb2-854c-88e43ef9ab7f"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("70b5c5c3-2267-4f7c-b0f9-7ecc952e04a6"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("80562f50-8a7d-4bcd-8971-6d856bbcbb7f"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("d4d7c0e3-efcf-4dd2-86e7-17d69fda8c75"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("ea003a99-4755-4c19-b126-c5cffbc65088"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("8f76de0b-114a-4df8-a93d-cec927d06a3c"), new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263") },
                    { new Guid("28126ffd-51c2-4201-939c-b64e3df43b9d"), new Guid("11d14a89-3a93-4d39-a94f-82b823f0d4ce") }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263"), new Guid("c41a7761-6645-4e2c-b99d-f9e767b9ac77") },
                    { new Guid("11d14a89-3a93-4d39-a94f-82b823f0d4ce"), new Guid("065e903e-6f7b-42b8-b807-0c4197f9d1bc") },
                    { new Guid("f22bce18-06ec-474a-b9af-a9de2a7b8263"), new Guid("4b6d9e45-626d-489a-a8cf-d32d36583af4") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserId",
                table: "Account",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleList_CategoryId",
                table: "ArticleList",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Backpack_CharacterId",
                table: "Backpack",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Backpack_ItemId",
                table: "Backpack",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_AccountId",
                table: "Character",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_CharacteristicsId",
                table: "Character",
                column: "CharacteristicsId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_RaceId",
                table: "Character",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristic_CharacteristicInfoId",
                table: "Characteristic",
                column: "CharacteristicInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageTypeList_WeaponId",
                table: "DamageTypeList",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_Dices_DId",
                table: "Dices",
                column: "DId");

            migrationBuilder.CreateIndex(
                name: "IX_Effect_ArticleId",
                table: "Effect",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCharacteristic_CharismaId",
                table: "GroupCharacteristic",
                column: "CharismaId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCharacteristic_ConstitutionId",
                table: "GroupCharacteristic",
                column: "ConstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCharacteristic_DexterityId",
                table: "GroupCharacteristic",
                column: "DexterityId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCharacteristic_IntelligenceId",
                table: "GroupCharacteristic",
                column: "IntelligenceId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCharacteristic_StrengthId",
                table: "GroupCharacteristic",
                column: "StrengthId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCharacteristic_WisdomId",
                table: "GroupCharacteristic",
                column: "WisdomId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ArticleId",
                table: "Items",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Race_SizeId",
                table: "Race",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                table: "RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsProducts_ItemId",
                table: "ShopsProducts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsProducts_ShopId",
                table: "ShopsProducts",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponItemProperty_WeaponId",
                table: "WeaponItemProperty",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_AmmunitionId",
                table: "WeaponList",
                column: "AmmunitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_ArticleId",
                table: "WeaponList",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_LargeSizeDamageId",
                table: "WeaponList",
                column: "LargeSizeDamageId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_MediumSizeDamageId",
                table: "WeaponList",
                column: "MediumSizeDamageId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_SmallSizeDamageId",
                table: "WeaponList",
                column: "SmallSizeDamageId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponList_WeaponTypeId",
                table: "WeaponList",
                column: "WeaponTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Backpack");

            migrationBuilder.DropTable(
                name: "DamageTypeList");

            migrationBuilder.DropTable(
                name: "Effect");

            migrationBuilder.DropTable(
                name: "RoleClaim");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "ShopsProducts");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "WeaponItemProperty");

            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "WeaponList");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "GroupCharacteristic");

            migrationBuilder.DropTable(
                name: "Race");

            migrationBuilder.DropTable(
                name: "Ammunition");

            migrationBuilder.DropTable(
                name: "Dices");

            migrationBuilder.DropTable(
                name: "WeaponTypeList");

            migrationBuilder.DropTable(
                name: "ArticleList");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Characteristic");

            migrationBuilder.DropTable(
                name: "RaceSize");

            migrationBuilder.DropTable(
                name: "BaseDice");

            migrationBuilder.DropTable(
                name: "CategoryList");

            migrationBuilder.DropTable(
                name: "CharacteristicInfo");
        }
    }
}
