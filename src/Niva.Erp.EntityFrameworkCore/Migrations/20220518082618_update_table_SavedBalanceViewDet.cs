using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_SavedBalanceViewDet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedBalanceViewDet_Account_AccountId",
                table: "SavedBalanceViewDet");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "SavedBalanceViewDet",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "CursValutar",
                table: "Invoices",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedBalanceViewDet_Account_AccountId",
                table: "SavedBalanceViewDet",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedBalanceViewDet_Account_AccountId",
                table: "SavedBalanceViewDet");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "SavedBalanceViewDet",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CursValutar",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedBalanceViewDet_Account_AccountId",
                table: "SavedBalanceViewDet",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
