using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_Invoices_InvoiceDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonedaPlataId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareTotalaFactura",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareTotalaDetaliu",
                table: "InvoiceDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_MonedaPlataId",
                table: "Invoices",
                column: "MonedaPlataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Currency_MonedaPlataId",
                table: "Invoices",
                column: "MonedaPlataId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Currency_MonedaPlataId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_MonedaPlataId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "MonedaPlataId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ValoareTotalaFactura",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ValoareTotalaDetaliu",
                table: "InvoiceDetails");
        }
    }
}
