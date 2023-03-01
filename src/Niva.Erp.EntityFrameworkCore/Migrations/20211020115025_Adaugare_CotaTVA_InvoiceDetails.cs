using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Adaugare_CotaTVA_InvoiceDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CotaTVA_Id",
                table: "InvoiceDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_CotaTVA_Id",
                table: "InvoiceDetails",
                column: "CotaTVA_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_CotaTVA_CotaTVA_Id",
                table: "InvoiceDetails",
                column: "CotaTVA_Id",
                principalTable: "CotaTVA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_CotaTVA_CotaTVA_Id",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_CotaTVA_Id",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CotaTVA_Id",
                table: "InvoiceDetails");
        }
    }
}
