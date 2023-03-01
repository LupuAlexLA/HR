using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class MigSumeReinvestite1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DobandaTotala",
                table: "BVC_VenitTitluCFReinv");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DobandaTotala",
                table: "BVC_VenitTitluCFReinv",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
