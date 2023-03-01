using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateObiecteDeInventar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectItem_Account_AssetAccountId",
                table: "InvObjectItem");

            migrationBuilder.DropIndex(
                name: "IX_InvObjectItem_AssetAccountId",
                table: "InvObjectItem");

            migrationBuilder.DropColumn(
                name: "AssetAccountId",
                table: "InvObjectItem");

            migrationBuilder.AddColumn<int>(
                name: "InvObjectAccountId",
                table: "InvObjectItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperationType",
                table: "InvObjectItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_InvObjectAccountId",
                table: "InvObjectItem",
                column: "InvObjectAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectItem_Account_InvObjectAccountId",
                table: "InvObjectItem",
                column: "InvObjectAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectItem_Account_InvObjectAccountId",
                table: "InvObjectItem");

            migrationBuilder.DropIndex(
                name: "IX_InvObjectItem_InvObjectAccountId",
                table: "InvObjectItem");

            migrationBuilder.DropColumn(
                name: "InvObjectAccountId",
                table: "InvObjectItem");

            migrationBuilder.DropColumn(
                name: "OperationType",
                table: "InvObjectItem");

            migrationBuilder.AddColumn<int>(
                name: "AssetAccountId",
                table: "InvObjectItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_AssetAccountId",
                table: "InvObjectItem",
                column: "AssetAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectItem_Account_AssetAccountId",
                table: "InvObjectItem",
                column: "AssetAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
