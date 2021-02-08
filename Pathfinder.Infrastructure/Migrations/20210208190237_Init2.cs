using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Pathfinder.Infrastructure.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characteristic_CharacteristicInfo_CharacteristicInfoId",
                table: "Characteristic");

            migrationBuilder.DropForeignKey(
                name: "FK_Race_RaceSize_SizeId",
                table: "Race");

            migrationBuilder.DropIndex(
                name: "IX_Characteristic_CharacteristicInfoId",
                table: "Characteristic");

            migrationBuilder.DropColumn(
                name: "CharacteristicInfoId",
                table: "Characteristic");

            migrationBuilder.RenameColumn(
                name: "HaveNigthVision",
                table: "Race",
                newName: "IsNightVision");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RaceSize",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "SizeId",
                table: "Race",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "7511af33-a1b7-4fad-b4c2-3f6c50710aca");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "e8b9631e-1a0c-4fb5-a5ef-278b9c54ab20");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "f1778957-7c22-4d41-97c3-b73f6ea1b19f");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "c9ae7376-ef5e-4a31-968d-42425e5da087");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "f0f87b87-82ba-4d3b-9d02-e9a4fb25f753");

            migrationBuilder.AddForeignKey(
                name: "FK_Race_RaceSize_SizeId",
                table: "Race",
                column: "SizeId",
                principalTable: "RaceSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Race_RaceSize_SizeId",
                table: "Race");

            migrationBuilder.RenameColumn(
                name: "IsNightVision",
                table: "Race",
                newName: "HaveNigthVision");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RaceSize",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "SizeId",
                table: "Race",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CharacteristicInfoId",
                table: "Characteristic",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "513f9008-d351-465e-ac89-26ec57d0aa1f");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "81b847b9-b7fe-4529-979f-beefccbe7909");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "52d4c5b7-b959-4b76-9ed1-85bd4a03d0c6");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "591ad155-1724-4f37-affa-aba758722e49");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "36483718-c8ba-4fe9-b644-6603de9db8db");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristic_CharacteristicInfoId",
                table: "Characteristic",
                column: "CharacteristicInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characteristic_CharacteristicInfo_CharacteristicInfoId",
                table: "Characteristic",
                column: "CharacteristicInfoId",
                principalTable: "CharacteristicInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Race_RaceSize_SizeId",
                table: "Race",
                column: "SizeId",
                principalTable: "RaceSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
