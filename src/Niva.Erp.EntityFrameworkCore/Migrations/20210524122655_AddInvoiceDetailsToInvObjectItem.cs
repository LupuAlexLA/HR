using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddInvoiceDetailsToInvObjectItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceDetailsId",
                table: "InvObjectItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectItem_InvoiceDetailsId",
                table: "InvObjectItem",
                column: "InvoiceDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectItem_InvoiceDetails_InvoiceDetailsId",
                table: "InvObjectItem",
                column: "InvoiceDetailsId",
                principalTable: "InvoiceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectItem_InvoiceDetails_InvoiceDetailsId",
                table: "InvObjectItem");

            migrationBuilder.DropIndex(
                name: "IX_InvObjectItem_InvoiceDetailsId",
                table: "InvObjectItem");

            migrationBuilder.DropColumn(
                name: "InvoiceDetailsId",
                table: "InvObjectItem");
        }
    }
}
