using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class VenitCheltAddFormId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormularId",
                table: "BVC_VenitCheltuieli",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitCheltuieli_FormularId",
                table: "BVC_VenitCheltuieli",
                column: "FormularId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitCheltuieli_BVC_Formular_FormularId",
                table: "BVC_VenitCheltuieli",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitCheltuieli_BVC_Formular_FormularId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitCheltuieli_FormularId",
                table: "BVC_VenitCheltuieli");

            migrationBuilder.DropColumn(
                name: "FormularId",
                table: "BVC_VenitCheltuieli");
        }
    }
}
