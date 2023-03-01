using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addIncludMonedaToAccountConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IncludMoneda",
                table: "AccountConfig",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails");

            migrationBuilder.DropColumn(
                name: "IncludMoneda",
                table: "AccountConfig");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id");
        }
    }
}
