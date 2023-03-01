using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updareRedistribuire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.AlterColumn<int>(
                name: "BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId",
                principalTable: "BVC_PaapRedistribuire",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.AlterColumn<int>(
                name: "BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PaapRedistribuireDetalii_BVC_PaapRedistribuire_BVC_PaapRedistribuireId",
                table: "BVC_PaapRedistribuireDetalii",
                column: "BVC_PaapRedistribuireId",
                principalTable: "BVC_PaapRedistribuire",
                principalColumn: "Id");
        }
    }
}
