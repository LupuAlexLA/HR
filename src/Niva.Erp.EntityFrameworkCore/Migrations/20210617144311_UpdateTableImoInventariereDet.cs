using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableImoInventariereDet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoInventariereDet_ImoInventariere_ImoInventariereId",
                table: "ImoInventariereDet");

            migrationBuilder.AlterColumn<int>(
                name: "ImoInventariereId",
                table: "ImoInventariereDet",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoInventariereDet_ImoInventariere_ImoInventariereId",
                table: "ImoInventariereDet",
                column: "ImoInventariereId",
                principalTable: "ImoInventariere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoInventariereDet_ImoInventariere_ImoInventariereId",
                table: "ImoInventariereDet");

            migrationBuilder.AlterColumn<int>(
                name: "ImoInventariereId",
                table: "ImoInventariereDet",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ImoInventariereDet_ImoInventariere_ImoInventariereId",
                table: "ImoInventariereDet",
                column: "ImoInventariereId",
                principalTable: "ImoInventariere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
