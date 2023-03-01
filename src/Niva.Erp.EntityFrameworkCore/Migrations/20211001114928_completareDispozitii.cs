using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class completareDispozitii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActIdentitate",
                table: "Dispositions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumePrenume",
                table: "Dispositions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipDoc",
                table: "Dispositions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActIdentitate",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "NumePrenume",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "TipDoc",
                table: "Dispositions");
        }
    }
}
