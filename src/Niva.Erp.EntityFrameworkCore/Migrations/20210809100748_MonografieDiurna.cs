using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class MonografieDiurna : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScopDeplasareType",
                table: "DiurnaZi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScopDeplasareType",
                table: "DiurnaLegala",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiurnaType",
                table: "AccountConfig",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScopDeplasareType",
                table: "AccountConfig",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScopDeplasareType",
                table: "DiurnaZi");

            migrationBuilder.DropColumn(
                name: "ScopDeplasareType",
                table: "DiurnaLegala");

            migrationBuilder.DropColumn(
                name: "DiurnaType",
                table: "AccountConfig");

            migrationBuilder.DropColumn(
                name: "ScopDeplasareType",
                table: "AccountConfig");
        }
    }
}
