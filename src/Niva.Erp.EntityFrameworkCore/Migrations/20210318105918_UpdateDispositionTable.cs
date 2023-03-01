using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateDispositionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Invoices_InoiceId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_InoiceId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "InoiceId",
                table: "Dispositions");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Dispositions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InvoiceId",
                table: "Dispositions",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Invoices_InvoiceId",
                table: "Dispositions",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Invoices_InvoiceId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_InvoiceId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Dispositions");

            migrationBuilder.AddColumn<int>(
                name: "InoiceId",
                table: "Dispositions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InoiceId",
                table: "Dispositions",
                column: "InoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Invoices_InoiceId",
                table: "Dispositions",
                column: "InoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
