using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addgarantiestate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumutId",
                table: "Garantie",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Garantie");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumutId",
                table: "Garantie",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
