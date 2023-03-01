using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addInvoiceElementsDetailsToDisopsitionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DocumentDate",
                table: "Dispositions",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsId",
                table: "Dispositions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InvoiceElementsDetailsId",
                table: "Dispositions",
                column: "InvoiceElementsDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_InvoiceElementsDetails_InvoiceElementsDetailsId",
                table: "Dispositions",
                column: "InvoiceElementsDetailsId",
                principalTable: "InvoiceElementsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_InvoiceElementsDetails_InvoiceElementsDetailsId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_InvoiceElementsDetailsId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DocumentDate",
                table: "Dispositions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
