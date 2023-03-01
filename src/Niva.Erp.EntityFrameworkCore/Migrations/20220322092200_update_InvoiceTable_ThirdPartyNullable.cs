using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_InvoiceTable_ThirdPartyNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Persons_ThirdPartyId", 
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "ThirdPartyId",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Persons_ThirdPartyId",
                table: "Invoices",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Persons_ThirdPartyId",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "ThirdPartyId",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Persons_ThirdPartyId",
                table: "Invoices",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
