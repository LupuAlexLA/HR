using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateNotificareTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserVizualizare",
                table: "Notificare");

            migrationBuilder.AddColumn<int>(
                name: "UserVizualizareId",
                table: "Notificare",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserVizualizareId",
                table: "Notificare");

            migrationBuilder.AddColumn<int>(
                name: "UserVizualizare",
                table: "Notificare",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
