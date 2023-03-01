using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_Invoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CursValutar",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DecontareInLei",
                table: "Invoices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CursValutar",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DecontareInLei",
                table: "Invoices");
        }
    }
}
