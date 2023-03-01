using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class restul : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Invoices_InvoiceId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_InvoiceId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "InvoicesId",
                table: "Dispositions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CotaTVA_Id",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocalCurrencyId",
                table: "BVC_PAAP",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentOrderInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PaymentOrderId = table.Column<int>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false),
                    PayedValue = table.Column<decimal>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrderInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrderInvoice_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentOrderInvoice_PaymentOrders_PaymentOrderId",
                        column: x => x.PaymentOrderId,
                        principalTable: "PaymentOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InvoicesId",
                table: "Dispositions",
                column: "InvoicesId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CotaTVA_Id",
                table: "Contracts",
                column: "CotaTVA_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderInvoice_InvoiceId",
                table: "PaymentOrderInvoice",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderInvoice_PaymentOrderId",
                table: "PaymentOrderInvoice",
                column: "PaymentOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_CotaTVA_CotaTVA_Id",
                table: "Contracts",
                column: "CotaTVA_Id",
                principalTable: "CotaTVA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Invoices_InvoicesId",
                table: "Dispositions",
                column: "InvoicesId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_CotaTVA_CotaTVA_Id",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Invoices_InvoicesId",
                table: "Dispositions");

            migrationBuilder.DropTable(
                name: "PaymentOrderInvoice");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_InvoicesId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_CotaTVA_Id",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "InvoicesId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "CotaTVA_Id",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "LocalCurrencyId",
                table: "BVC_PAAP");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Dispositions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VAT",
                table: "Contracts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InvoiceId",
                table: "Dispositions",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Invoices_InvoiceId",
                table: "Dispositions",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
