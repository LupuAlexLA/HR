using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class contaoperationcomisioane : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContaOperationDetailId",
                table: "DateComision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContaOperationId",
                table: "DateComision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DateComision_ContaOperationDetailId",
                table: "DateComision",
                column: "ContaOperationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DateComision_ContaOperationId",
                table: "DateComision",
                column: "ContaOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DateComision_OperationsDetails_ContaOperationDetailId",
                table: "DateComision",
                column: "ContaOperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DateComision_Operations_ContaOperationId",
                table: "DateComision",
                column: "ContaOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateComision_OperationsDetails_ContaOperationDetailId",
                table: "DateComision");

            migrationBuilder.DropForeignKey(
                name: "FK_DateComision_Operations_ContaOperationId",
                table: "DateComision");

            migrationBuilder.DropIndex(
                name: "IX_DateComision_ContaOperationDetailId",
                table: "DateComision");

            migrationBuilder.DropIndex(
                name: "IX_DateComision_ContaOperationId",
                table: "DateComision");

            migrationBuilder.DropColumn(
                name: "ContaOperationDetailId",
                table: "DateComision");

            migrationBuilder.DropColumn(
                name: "ContaOperationId",
                table: "DateComision");
        }
    }
}
