using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Update_Table_CupiuriDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CupiuriItem_CupiuriInit_CupiuriInitId",
                table: "CupiuriItem");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "CupiuriDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CupiuriItem_CupiuriInit_CupiuriInitId",
                table: "CupiuriItem",
                column: "CupiuriInitId",
                principalTable: "CupiuriInit",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CupiuriItem_CupiuriInit_CupiuriInitId",
                table: "CupiuriItem");

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "CupiuriDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddForeignKey(
                name: "FK_CupiuriItem_CupiuriInit_CupiuriInitId",
                table: "CupiuriItem",
                column: "CupiuriInitId",
                principalTable: "CupiuriInit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
