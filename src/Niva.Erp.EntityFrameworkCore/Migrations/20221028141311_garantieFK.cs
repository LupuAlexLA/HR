using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class garantieFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GarantieId",
                table: "OperatieGarantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OperatieGarantie_GarantieId",
                table: "OperatieGarantie",
                column: "GarantieId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperatieGarantie_Garantie_GarantieId",
                table: "OperatieGarantie",
                column: "GarantieId",
                principalTable: "Garantie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperatieGarantie_Garantie_GarantieId",
                table: "OperatieGarantie");

            migrationBuilder.DropIndex(
                name: "IX_OperatieGarantie_GarantieId",
                table: "OperatieGarantie");

            migrationBuilder.DropColumn(
                name: "GarantieId",
                table: "OperatieGarantie");
        }
    }
}
