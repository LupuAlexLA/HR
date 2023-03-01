using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class modificare_BVC_PAAP_InvoiceDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_InvoiceDetails_InvoiceDetailsId_BVC_PAAPId",
                table: "BVC_PAAP_InvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_InvoiceDetails_InvoiceDetailsId",
                table: "BVC_PAAP_InvoiceDetails",
                column: "InvoiceDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_InvoiceDetails_InvoiceDetailsId",
                table: "BVC_PAAP_InvoiceDetails");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_InvoiceDetails_InvoiceDetailsId_BVC_PAAPId",
                table: "BVC_PAAP_InvoiceDetails",
                columns: new[] { "InvoiceDetailsId", "BVC_PAAPId" },
                unique: true);
        }
    }
}
