using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_LichidBenzi_DurataInLuni : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DurataMinima",
                table: "LichidBenzi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DurataMaxima",
                table: "LichidBenzi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DurataInLuniMaxima",
                table: "LichidBenzi",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurataInLuniMinima",
                table: "LichidBenzi",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurataInLuniMaxima",
                table: "LichidBenzi");

            migrationBuilder.DropColumn(
                name: "DurataInLuniMinima",
                table: "LichidBenzi");

            migrationBuilder.AlterColumn<int>(
                name: "DurataMinima",
                table: "LichidBenzi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurataMaxima",
                table: "LichidBenzi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
