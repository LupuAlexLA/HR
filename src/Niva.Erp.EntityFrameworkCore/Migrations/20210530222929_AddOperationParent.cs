using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddOperationParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseDirectAccount",
                table: "InvoiceElementsDetails");

            migrationBuilder.AddColumn<int>(
                name: "OperationParentId",
                table: "Operations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OperationParentId",
                table: "Operations",
                column: "OperationParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Operations_OperationParentId",
                table: "Operations",
                column: "OperationParentId",
                principalTable: "Operations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Operations_OperationParentId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_OperationParentId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OperationParentId",
                table: "Operations");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseDirectAccount",
                table: "InvoiceElementsDetails",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
