using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updatePaap_RedistribuireTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SumaPierduta",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "SumaPlatita",
                table: "BVC_PaapRedistribuire",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PaapRedistribuire_PaapCarePrimesteId",
                table: "BVC_PaapRedistribuire",
                column: "PaapCarePrimesteId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuire_BVC_PAAP_PaapCarePrimesteId",
                table: "BVC_PaapRedistribuire",
                column: "PaapCarePrimesteId",
                principalTable: "BVC_PAAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuire_BVC_PAAP_PaapCarePrimesteId",
                table: "BVC_PaapRedistribuire");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PaapRedistribuire_PaapCarePrimesteId",
                table: "BVC_PaapRedistribuire");

            migrationBuilder.AlterColumn<double>(
                name: "SumaPierduta",
                table: "BVC_PaapRedistribuireDetalii",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "SumaPlatita",
                table: "BVC_PaapRedistribuire",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
