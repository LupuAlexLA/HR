using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addFK_BVC_PaapRedistribuireDetalii1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropColumn(
                name: "PaapCarePrimesteId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.AddColumn<int>(
                name: "BVC_PaapRedistribuireId1",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuireId1",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId",
                principalTable: "BVC_PaapRedistribuire",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId1",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId1",
                principalTable: "BVC_PaapRedistribuire",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId1",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuireId1",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropColumn(
                name: "BVC_PaapRedistribuireId1",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.AddColumn<int>(
                name: "PaapCarePrimesteId",
                table: "BVC_PaapRedistribuireDetalii",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId",
                principalTable: "BVC_PaapRedistribuire",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
