using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class contaoperdetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContaOperationDetailId",
                table: "Dobanda",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dobanda_ContaOperationDetailId",
                table: "Dobanda",
                column: "ContaOperationDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dobanda_OperationsDetails_ContaOperationDetailId",
                table: "Dobanda",
                column: "ContaOperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dobanda_OperationsDetails_ContaOperationDetailId",
                table: "Dobanda");

            migrationBuilder.DropIndex(
                name: "IX_Dobanda_ContaOperationDetailId",
                table: "Dobanda");

            migrationBuilder.DropColumn(
                name: "ContaOperationDetailId",
                table: "Dobanda");
        }
    }
}
