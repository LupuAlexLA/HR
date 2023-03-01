using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class InvoicesDocumentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoicesDocumentType",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoicesDocumentType",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
