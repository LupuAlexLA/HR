using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class PrelungireAutomataContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrLuniPrelungire",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrelungireAutomata",
                table: "Contracts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrLuniPrelungire",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PrelungireAutomata",
                table: "Contracts");
        }
    }
}
