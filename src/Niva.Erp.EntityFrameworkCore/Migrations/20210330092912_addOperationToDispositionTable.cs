using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addOperationToDispositionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationId",
                table: "Dispositions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_OperationId",
                table: "Dispositions",
                column: "OperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Operations_OperationId",
                table: "Dispositions",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Operations_OperationId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_OperationId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "OperationId",
                table: "Dispositions");
        }
    }
}
