using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTragere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Comision",
                table: "Tragere",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaImprumutata",
                table: "Tragere",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TipTragere",
                table: "Tragere",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comision",
                table: "Tragere");

            migrationBuilder.DropColumn(
                name: "SumaImprumutata",
                table: "Tragere");

            migrationBuilder.DropColumn(
                name: "TipTragere",
                table: "Tragere");
        }
    }
}
