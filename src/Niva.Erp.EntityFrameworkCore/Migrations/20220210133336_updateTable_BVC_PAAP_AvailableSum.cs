using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTable_BVC_PAAP_AvailableSum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_Departament_DepartamentId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_Departament_DepartamentId",
                table: "BVC_PAAP_AvailableSum",
                column: "DepartamentId",
                principalTable: "Departament",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsCategoryId",
                principalTable: "InvoiceElementsDetailsCategory",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_Departament_DepartamentId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_Departament_DepartamentId",
                table: "BVC_PAAP_AvailableSum",
                column: "DepartamentId",
                principalTable: "Departament",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsCategoryId",
                principalTable: "InvoiceElementsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
