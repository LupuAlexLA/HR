using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateBVC_PaapRedistribDetalii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaapCarePrimesteId",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PaapRedistribuireDetalii_PaapCarePierdeId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "PaapCarePierdeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PAAP_PaapCarePierdeId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "PaapCarePierdeId",
                principalTable: "BVC_PAAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PAAP_PaapCarePierdeId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PaapRedistribuireDetalii_PaapCarePierdeId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropColumn(
                name: "PaapCarePrimesteId",
                table: "BVC_PaapRedistribuireDetalii");
        }
    }
}
