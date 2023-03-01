using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTableNotificare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalariatId",
                table: "Notificare");

            migrationBuilder.AddColumn<int>(
                name: "IdPersonal",
                table: "Notificare",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPersonal",
                table: "Notificare");

            migrationBuilder.AddColumn<int>(
                name: "SalariatId",
                table: "Notificare",
                type: "int",
                nullable: true);
        }
    }
}
