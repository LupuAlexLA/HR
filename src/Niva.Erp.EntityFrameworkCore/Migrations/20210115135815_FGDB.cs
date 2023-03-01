using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class FGDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalyticAccount",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SyntheticAccount",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Account",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyntheticAccountId",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_SyntheticAccountId",
                table: "Account",
                column: "SyntheticAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Account_SyntheticAccountId",
                table: "Account",
                column: "SyntheticAccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Account_SyntheticAccountId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_SyntheticAccountId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SyntheticAccountId",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "AnalyticAccount",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyntheticAccount",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
