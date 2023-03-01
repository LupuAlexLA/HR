using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ContaOperToDecont : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "Decont",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decont_OperationId",
                table: "Decont",
                column: "OperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decont_Operations_OperationId",
                table: "Decont",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decont_Operations_OperationId",
                table: "Decont");

            migrationBuilder.DropIndex(
                name: "IX_Decont_OperationId",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "Decont");
        }
    }
}
