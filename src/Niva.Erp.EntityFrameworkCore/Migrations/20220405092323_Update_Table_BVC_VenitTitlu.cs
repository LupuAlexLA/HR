using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Update_Table_BVC_VenitTitlu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Reinvestit",
                table: "BVC_VenitTitlu",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Selectat",
                table: "BVC_VenitTitlu",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reinvestit",
                table: "BVC_VenitTitlu");

            migrationBuilder.DropColumn(
                name: "Selectat",
                table: "BVC_VenitTitlu");
        }
    }
}
