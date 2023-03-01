using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class operationDetailsOperGenerate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperGenerateId",
                table: "OperationsDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationsDetails_OperGenerateId",
                table: "OperationsDetails",
                column: "OperGenerateId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_OperGenerate_OperGenerateId",
                table: "OperationsDetails",
                column: "OperGenerateId",
                principalTable: "OperGenerate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_OperGenerate_OperGenerateId",
                table: "OperationsDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationsDetails_OperGenerateId",
                table: "OperationsDetails");

            migrationBuilder.DropColumn(
                name: "OperGenerateId",
                table: "OperationsDetails");
        }
    }
}
