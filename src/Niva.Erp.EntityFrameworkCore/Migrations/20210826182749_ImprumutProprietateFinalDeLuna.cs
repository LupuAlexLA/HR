using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ImprumutProprietateFinalDeLuna : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImprumuturiStare",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isFinalDeLuna",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImprumuturiStare",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "isFinalDeLuna",
                table: "Imprumuturi");
        }
    }
}
