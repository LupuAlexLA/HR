using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTableInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ActivityTypeId",
                table: "Invoices",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_ActivityTypes_ActivityTypeId",
                table: "Invoices",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_ActivityTypes_ActivityTypeId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ActivityTypeId",
                table: "Invoices");
        }
    }
}
