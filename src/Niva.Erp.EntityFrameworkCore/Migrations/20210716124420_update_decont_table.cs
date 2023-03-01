using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_decont_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diurna",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "DiurnaImopzabila",
                table: "Decont",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiurnaLegala",
                table: "Decont",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiurnaZi",
                table: "Decont",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NrZile",
                table: "Decont",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiurnaImopzabila",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "DiurnaLegala",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "DiurnaZi",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "NrZile",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "Diurna",
                table: "Decont",
                type: "int",
                nullable: true);
        }
    }
}
