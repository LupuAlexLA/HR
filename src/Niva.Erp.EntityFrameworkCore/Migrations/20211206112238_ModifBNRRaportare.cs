using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ModifBNRRaportare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId",
                table: "BNR_RaportareRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId1",
                table: "BNR_RaportareRand");

            migrationBuilder.DropIndex(
                name: "IX_BNR_RaportareRand_BNR_RaportareId1",
                table: "BNR_RaportareRand");

            migrationBuilder.DropColumn(
                name: "BNR_RaportareId1",
                table: "BNR_RaportareRand");

            migrationBuilder.AddForeignKey(
                name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId",
                table: "BNR_RaportareRand",
                column: "BNR_RaportareId",
                principalTable: "BNR_Raportare",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId",
                table: "BNR_RaportareRand");

            migrationBuilder.AddColumn<int>(
                name: "BNR_RaportareId1",
                table: "BNR_RaportareRand",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BNR_RaportareRand_BNR_RaportareId1",
                table: "BNR_RaportareRand",
                column: "BNR_RaportareId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId",
                table: "BNR_RaportareRand",
                column: "BNR_RaportareId",
                principalTable: "BNR_Raportare",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId1",
                table: "BNR_RaportareRand",
                column: "BNR_RaportareId1",
                principalTable: "BNR_Raportare",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
