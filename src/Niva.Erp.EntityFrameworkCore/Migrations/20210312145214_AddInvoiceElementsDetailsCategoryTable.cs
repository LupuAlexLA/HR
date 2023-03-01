using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddInvoiceElementsDetailsCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoiceElementsDetailsCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CategoryElementDetName = table.Column<string>(maxLength: 1000, nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceElementsDetailsCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails",
                column: "InvoiceElementsDetailsCategoryId",
                unique: true,
                filter: "[InvoiceElementsDetailsCategoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceElementsDetails_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails",
                column: "InvoiceElementsDetailsCategoryId",
                principalTable: "InvoiceElementsDetailsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceElementsDetails_InvoiceElementsDetailsCategory_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails");

            migrationBuilder.DropTable(
                name: "InvoiceElementsDetailsCategory");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceElementsDetails_InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceElementsDetailsCategoryId",
                table: "InvoiceElementsDetails");
        }
    }
}
