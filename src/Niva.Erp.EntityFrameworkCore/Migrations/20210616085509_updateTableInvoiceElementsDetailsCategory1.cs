using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTableInvoiceElementsDetailsCategory1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cate4goryType",
                table: "InvoiceElementsDetailsCategory");

            migrationBuilder.AddColumn<int>(
                name: "CategoryType",
                table: "InvoiceElementsDetailsCategory",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "InvoiceElementsDetailsCategory");

            migrationBuilder.AddColumn<int>(
                name: "Cate4goryType",
                table: "InvoiceElementsDetailsCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
