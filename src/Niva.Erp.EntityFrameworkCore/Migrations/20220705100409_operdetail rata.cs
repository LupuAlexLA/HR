using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class operdetailrata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContaOperationDetailId",
                table: "Rate",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rate_ContaOperationDetailId",
                table: "Rate",
                column: "ContaOperationDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_OperationsDetails_ContaOperationDetailId",
                table: "Rate",
                column: "ContaOperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rate_OperationsDetails_ContaOperationDetailId",
                table: "Rate");

            migrationBuilder.DropIndex(
                name: "IX_Rate_ContaOperationDetailId",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "ContaOperationDetailId",
                table: "Rate");
        }
    }
}
