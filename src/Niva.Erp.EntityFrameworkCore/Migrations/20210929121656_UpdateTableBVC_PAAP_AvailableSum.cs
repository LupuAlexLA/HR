using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableBVC_PAAP_AvailableSum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_AvailableSum_InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedYear",
                table: "BVC_PAAP_AvailableSum",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_AvailableSum_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsCategoryId",
                principalTable: "InvoiceElementsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_AvailableSum_InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropColumn(
                name: "ApprovedYear",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsCategoryId",
                table: "BVC_PAAP_AvailableSum");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_AvailableSum_InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_AvailableSum_InvoiceElementsDetails_InvoiceElementsDetailsId",
                table: "BVC_PAAP_AvailableSum",
                column: "InvoiceElementsDetailsId",
                principalTable: "InvoiceElementsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
