using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTable_SavedBalanceDetailsCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelRand",
                table: "SavedBalanceDetailsCurrencies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NivelRand",
                table: "SavedBalanceDetailsCurrencies",
                type: "int",
                nullable: true);
        }
    }
}
