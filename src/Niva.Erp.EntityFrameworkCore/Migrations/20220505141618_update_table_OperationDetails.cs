using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_OperationDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsCategoryId",
                table: "OperationsDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsId",
                table: "OperationsDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationsDetails_InvoiceElementsDetailsCategoryId",
                table: "OperationsDetails",
                column: "InvoiceElementsDetailsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationsDetails_InvoiceElementsDetailsId",
                table: "OperationsDetails",
                column: "InvoiceElementsDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "OperationsDetails",
                column: "InvoiceElementsDetailsCategoryId",
                principalTable: "InvoiceElementsDetailsCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_InvoiceElementsDetails_InvoiceElementsDetailsId",
                table: "OperationsDetails",
                column: "InvoiceElementsDetailsId",
                principalTable: "InvoiceElementsDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "OperationsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_InvoiceElementsDetails_InvoiceElementsDetailsId",
                table: "OperationsDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationsDetails_InvoiceElementsDetailsCategoryId",
                table: "OperationsDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationsDetails_InvoiceElementsDetailsId",
                table: "OperationsDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsCategoryId",
                table: "OperationsDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsId",
                table: "OperationsDetails");
        }
    }
}
