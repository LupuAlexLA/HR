using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVCPrevLunaStartEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthEnd",
                table: "BVC_BugetPrev",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthStart",
                table: "BVC_BugetPrev",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthEnd",
                table: "BVC_BugetPrev");

            migrationBuilder.DropColumn(
                name: "MonthStart",
                table: "BVC_BugetPrev");
        }
    }
}
