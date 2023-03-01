using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateInvoiceElementsDetailCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails",
                column: "InvoiceElementsDetailsCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails",
                column: "InvoiceElementsDetailsCategoryId",
                unique: true,
                filter: "[InvoiceElementsDetailsCategoryId] IS NOT NULL");
        }
    }
}
