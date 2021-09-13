using Microsoft.EntityFrameworkCore.Migrations;

namespace Pathfinder.Infrastructure.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleList_CategoryList_CategoryType1",
                table: "ArticleList");

            migrationBuilder.DropIndex(
                name: "IX_ArticleList_CategoryType1",
                table: "ArticleList");

            migrationBuilder.DropColumn(
                name: "CategoryType1",
                table: "ArticleList");

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
                name: "FK_ArticleList_CategoryList_CategoryType",
                table: "ArticleList",
                column: "CategoryType",
                principalTable: "CategoryList",
                principalColumn: "CategoryType",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleList_CategoryList_CategoryType",
                table: "ArticleList");

            migrationBuilder.DropIndex(
                name: "IX_ArticleList_CategoryType",
                table: "ArticleList");

            migrationBuilder.AddColumn<byte>(
                name: "CategoryType1",
                table: "ArticleList",
                type: "smallint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "0d1da455-7971-4c72-990d-83e78369f7f1");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "dd9bfd97-5ab7-48fb-84c3-c8b4bb849e22");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "55dfb6a0-3214-428a-af57-7a56c6dafd67");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "56a6cd0d-2d7b-4255-bd3d-a94280887f51");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "cecf67d3-b3ee-4562-b4b5-6727002c5cbc");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleList_CategoryType1",
                table: "ArticleList",
                column: "CategoryType1");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleList_CategoryList_CategoryType1",
                table: "ArticleList",
                column: "CategoryType1",
                principalTable: "CategoryList",
                principalColumn: "CategoryType",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
