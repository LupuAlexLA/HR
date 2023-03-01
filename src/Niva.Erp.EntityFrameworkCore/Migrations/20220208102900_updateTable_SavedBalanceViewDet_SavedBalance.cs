using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTable_SavedBalanceViewDet_SavedBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedBalanceViewDet_Balance_BalanceId",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropIndex(
                name: "IX_SavedBalanceViewDet_BalanceId",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropColumn(
                name: "BalanceId",
                table: "SavedBalanceViewDet");

            migrationBuilder.AddColumn<int>(
                name: "SavedBalanceId",
                table: "SavedBalanceViewDet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceViewDet_SavedBalanceId",
                table: "SavedBalanceViewDet",
                column: "SavedBalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedBalanceViewDet_SavedBalance_SavedBalanceId",
                table: "SavedBalanceViewDet",
                column: "SavedBalanceId",
                principalTable: "SavedBalance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedBalanceViewDet_SavedBalance_SavedBalanceId",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropIndex(
                name: "IX_SavedBalanceViewDet_SavedBalanceId",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropColumn(
                name: "SavedBalanceId",
                table: "SavedBalanceViewDet");

            migrationBuilder.AddColumn<int>(
                name: "BalanceId",
                table: "SavedBalanceViewDet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceViewDet_BalanceId",
                table: "SavedBalanceViewDet",
                column: "BalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedBalanceViewDet_Balance_BalanceId",
                table: "SavedBalanceViewDet",
                column: "BalanceId",
                principalTable: "Balance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
