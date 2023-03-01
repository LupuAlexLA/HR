using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class corectieSumaOperatieDobandaComision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SumaComision",
                table: "OperatieDobandaComision");

            migrationBuilder.DropColumn(
                name: "SumaDobanda",
                table: "OperatieDobandaComision");

            migrationBuilder.AddColumn<decimal>(
                name: "Suma",
                table: "OperatieDobandaComision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Suma",
                table: "OperatieDobandaComision");

            migrationBuilder.AddColumn<decimal>(
                name: "SumaComision",
                table: "OperatieDobandaComision",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaDobanda",
                table: "OperatieDobandaComision",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
