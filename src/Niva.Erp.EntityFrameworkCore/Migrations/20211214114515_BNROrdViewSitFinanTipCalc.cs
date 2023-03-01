using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BNROrdViewSitFinanTipCalc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SitFinanRowModCalc",
                table: "SitFinanRapConfigCorel",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderView",
                table: "BNR_AnexaDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SitFinanRowModCalc",
                table: "SitFinanRapConfigCorel");

            migrationBuilder.DropColumn(
                name: "OrderView",
                table: "BNR_AnexaDetails");
        }
    }
}
