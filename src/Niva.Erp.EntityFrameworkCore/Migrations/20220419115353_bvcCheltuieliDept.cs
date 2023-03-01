using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class bvcCheltuieliDept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartamentId",
                table: "BVC_Cheltuieli",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Cheltuieli_DepartamentId",
                table: "BVC_Cheltuieli",
                column: "DepartamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_Cheltuieli_Departament_DepartamentId",
                table: "BVC_Cheltuieli",
                column: "DepartamentId",
                principalTable: "Departament",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_Cheltuieli_Departament_DepartamentId",
                table: "BVC_Cheltuieli");

            migrationBuilder.DropIndex(
                name: "IX_BVC_Cheltuieli_DepartamentId",
                table: "BVC_Cheltuieli");

            migrationBuilder.DropColumn(
                name: "DepartamentId",
                table: "BVC_Cheltuieli");
        }
    }
}
