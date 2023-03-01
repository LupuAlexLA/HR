using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ModifResurseDetalii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CursValutar",
                table: "BVC_PrevResurseDetalii",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaInRon",
                table: "BVC_PrevResurseDetalii",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CursValutar",
                table: "BVC_PrevResurseDetalii");

            migrationBuilder.DropColumn(
                name: "SumaInRon",
                table: "BVC_PrevResurseDetalii");
        }
    }
}
