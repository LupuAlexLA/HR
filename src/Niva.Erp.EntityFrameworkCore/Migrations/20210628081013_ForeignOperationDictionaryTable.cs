using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ForeignOperationDictionaryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForeignOperationsDetails_ForeignOperation_ForeignOperationId1",
                table: "ForeignOperationsDetails");

            migrationBuilder.DropIndex(
                name: "IX_ForeignOperationsDetails_ForeignOperationId1",
                table: "ForeignOperationsDetails");

            migrationBuilder.DropColumn(
                name: "ForeignOperationId1",
                table: "ForeignOperationsDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForeignOperationId1",
                table: "ForeignOperationsDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsDetails_ForeignOperationId1",
                table: "ForeignOperationsDetails",
                column: "ForeignOperationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ForeignOperationsDetails_ForeignOperation_ForeignOperationId1",
                table: "ForeignOperationsDetails",
                column: "ForeignOperationId1",
                principalTable: "ForeignOperation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
