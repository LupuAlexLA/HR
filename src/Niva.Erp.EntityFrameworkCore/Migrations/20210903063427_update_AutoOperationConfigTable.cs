using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_AutoOperationConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperationsDetailId",
                table: "PaymentOrders");

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "AutoOperationConfig",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationConfig_DocumentTypeId",
                table: "AutoOperationConfig",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutoOperationConfig_DocumentType_DocumentTypeId",
                table: "AutoOperationConfig",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperationsDetailId",
                table: "PaymentOrders",
                column: "FgnOperationsDetailId",
                principalTable: "ForeignOperationsDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoOperationConfig_DocumentType_DocumentTypeId",
                table: "AutoOperationConfig");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperationsDetailId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_AutoOperationConfig_DocumentTypeId",
                table: "AutoOperationConfig");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "AutoOperationConfig");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperationsDetailId",
                table: "PaymentOrders",
                column: "FgnOperationsDetailId",
                principalTable: "ForeignOperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
