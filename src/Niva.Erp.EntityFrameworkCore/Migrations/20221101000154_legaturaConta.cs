using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class legaturaConta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContaOperationDetailId",
                table: "OperatieGarantie",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContaOperationId",
                table: "OperatieGarantie",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatieGarantie_ContaOperationDetailId",
                table: "OperatieGarantie",
                column: "ContaOperationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatieGarantie_ContaOperationId",
                table: "OperatieGarantie",
                column: "ContaOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperatieGarantie_OperationsDetails_ContaOperationDetailId",
                table: "OperatieGarantie",
                column: "ContaOperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperatieGarantie_Operations_ContaOperationId",
                table: "OperatieGarantie",
                column: "ContaOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperatieGarantie_OperationsDetails_ContaOperationDetailId",
                table: "OperatieGarantie");

            migrationBuilder.DropForeignKey(
                name: "FK_OperatieGarantie_Operations_ContaOperationId",
                table: "OperatieGarantie");

            migrationBuilder.DropIndex(
                name: "IX_OperatieGarantie_ContaOperationDetailId",
                table: "OperatieGarantie");

            migrationBuilder.DropIndex(
                name: "IX_OperatieGarantie_ContaOperationId",
                table: "OperatieGarantie");

            migrationBuilder.DropColumn(
                name: "ContaOperationDetailId",
                table: "OperatieGarantie");

            migrationBuilder.DropColumn(
                name: "ContaOperationId",
                table: "OperatieGarantie");
        }
    }
}
