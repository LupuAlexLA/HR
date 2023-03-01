using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BNR_Conturi_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodRand",
                table: "BNR_Conturi");

            migrationBuilder.AlterColumn<int>(
                name: "BNR_SectorId",
                table: "BNR_Conturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BNR_SectorId",
                table: "BNR_Conturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodRand",
                table: "BNR_Conturi",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
