using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class VenitCheltAddReinv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitCheltuieli_BVC_VenitTitlu_BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitCheltuieli_BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropColumn(
                name: "BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.AddColumn<int>(
                name: "BVC_VenitTitluCFReinvId",
                table: "BVC_VenitCheltuieli",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "BVC_VenitCheltuieli",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitCheltuieli_BVC_VenitTitluCFReinvId",
                table: "BVC_VenitCheltuieli",
                column: "BVC_VenitTitluCFReinvId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitCheltuieli_BVC_VenitTitluCFReinv_BVC_VenitTitluCFReinvId",
                table: "BVC_VenitCheltuieli",
                column: "BVC_VenitTitluCFReinvId",
                principalTable: "BVC_VenitTitluCFReinv",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitCheltuieli_BVC_VenitTitluCFReinv_BVC_VenitTitluCFReinvId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitCheltuieli_BVC_VenitTitluCFReinvId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropColumn(
                name: "BVC_VenitTitluCFReinvId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.AddColumn<int>(
                name: "BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitCheltuieli_BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli",
                column: "BVC_VenitTitluId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitCheltuieli_BVC_VenitTitlu_BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli",
                column: "BVC_VenitTitluId",
                principalTable: "BVC_VenitTitlu",
                principalColumn: "Id");
        }
    }
}
