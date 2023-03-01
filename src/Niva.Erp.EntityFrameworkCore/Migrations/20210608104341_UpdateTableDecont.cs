using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableDecont : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecontId",
                table: "InvoiceDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Decont",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_DecontId",
                table: "InvoiceDetails",
                column: "DecontId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Decont_DecontId",
                table: "InvoiceDetails",
                column: "DecontId",
                principalTable: "Decont",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Decont_DecontId",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_DecontId",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "DecontId",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Decont");
        }
    }
}
