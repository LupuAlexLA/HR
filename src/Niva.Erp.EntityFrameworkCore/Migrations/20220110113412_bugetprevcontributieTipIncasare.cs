using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class bugetprevcontributieTipIncasare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BankId",
                table: "BVC_BugetPrevContributie",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TipIncasare",
                table: "BVC_BugetPrevContributie",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipIncasare",
                table: "BVC_BugetPrevContributie");

            migrationBuilder.AlterColumn<int>(
                name: "BankId",
                table: "BVC_BugetPrevContributie",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
