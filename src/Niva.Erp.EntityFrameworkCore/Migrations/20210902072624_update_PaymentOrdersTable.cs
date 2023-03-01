using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_PaymentOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FgnOperationsDetailId",
                table: "PaymentOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_FgnOperationsDetailId",
                table: "PaymentOrders",
                column: "FgnOperationsDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperationsDetailId",
                table: "PaymentOrders",
                column: "FgnOperationsDetailId",
                principalTable: "ForeignOperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_ForeignOperationsDetails_FgnOperationsDetailId",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrders_FgnOperationsDetailId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "FgnOperationsDetailId",
                table: "PaymentOrders");
        }
    }
}
