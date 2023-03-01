using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateInvoiceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoicesDocumentType",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DocumentTypeId",
                table: "Invoices",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_DocumentType_DocumentTypeId",
                table: "Invoices",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_DocumentType_DocumentTypeId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_DocumentTypeId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoicesDocumentType",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
