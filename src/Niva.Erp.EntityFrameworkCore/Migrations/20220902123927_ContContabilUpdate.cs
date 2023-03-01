using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ContContabilUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContContabil",
                table: "Imprumuturi");

            migrationBuilder.AddColumn<int>(
                name: "ContContabilId",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContContabilId",
                table: "Imprumuturi");

            migrationBuilder.AddColumn<string>(
                name: "ContContabil",
                table: "Imprumuturi",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
