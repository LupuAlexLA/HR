using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Update_ExchangeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContaOperationId",
                table: "Exchange",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_ContaOperationId",
                table: "Exchange",
                column: "ContaOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchange_Operations_ContaOperationId",
                table: "Exchange",
                column: "ContaOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchange_Operations_ContaOperationId",
                table: "Exchange");

            migrationBuilder.DropIndex(
                name: "IX_Exchange_ContaOperationId",
                table: "Exchange");

            migrationBuilder.DropColumn(
                name: "ContaOperationId",
                table: "Exchange");
        }
    }
}
