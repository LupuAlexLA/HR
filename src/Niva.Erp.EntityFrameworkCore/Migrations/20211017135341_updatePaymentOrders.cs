using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updatePaymentOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Invoices_InvoicesId",
                table: "Dispositions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_Invoices_InvoiceId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrders_InvoiceId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_InvoicesId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "InvoicesId",
                table: "Dispositions");

            migrationBuilder.CreateTable(
                name: "DispositionInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DispositionId = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false),
                    PayedValue = table.Column<decimal>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispositionInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispositionInvoice_Dispositions_DispositionId",
                        column: x => x.DispositionId,
                        principalTable: "Dispositions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispositionInvoice_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispositionInvoice_DispositionId",
                table: "DispositionInvoice",
                column: "DispositionId");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionInvoice_InvoiceId",
                table: "DispositionInvoice",
                column: "InvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispositionInvoice");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "PaymentOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoicesId",
                table: "Dispositions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_InvoiceId",
                table: "PaymentOrders",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InvoicesId",
                table: "Dispositions",
                column: "InvoicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Invoices_InvoicesId",
                table: "Dispositions",
                column: "InvoicesId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_Invoices_InvoiceId",
                table: "PaymentOrders",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
