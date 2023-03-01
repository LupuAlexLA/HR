using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_InvoicesV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "MonedaFacturaId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_MonedaFacturaId",
                table: "Invoices",
                column: "MonedaFacturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Currency_MonedaFacturaId",
                table: "Invoices",
                column: "MonedaFacturaId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Currency_MonedaFacturaId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_MonedaFacturaId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "MonedaFacturaId",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "MonedaPlataId",
                table: "Invoices",
                type: "int",
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
    }
}
