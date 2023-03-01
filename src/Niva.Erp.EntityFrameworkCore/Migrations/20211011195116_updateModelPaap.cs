using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateModelPaap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValoareTotalaLei",
                table: "BVC_PAAP",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareTotalaValuta",
                table: "BVC_PAAP",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValoareTotalaLei",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "ValoareTotalaValuta",
                table: "BVC_PAAP");
        }
    }
}
