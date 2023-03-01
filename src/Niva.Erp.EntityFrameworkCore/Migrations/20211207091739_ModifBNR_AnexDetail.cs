using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ModifBNR_AnexDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormulaCresteri",
                table: "BNR_AnexaDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaReduceri",
                table: "BNR_AnexaDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipTitlu",
                table: "BNR_AnexaDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormulaCresteri",
                table: "BNR_AnexaDetails");

            migrationBuilder.DropColumn(
                name: "FormulaReduceri",
                table: "BNR_AnexaDetails");

            migrationBuilder.DropColumn(
                name: "TipTitlu",
                table: "BNR_AnexaDetails");
        }
    }
}
