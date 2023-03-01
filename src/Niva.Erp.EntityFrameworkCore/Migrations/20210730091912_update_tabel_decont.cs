using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_tabel_decont : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiurnaImopzabila",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "DiurnaImpozabila",
                table: "Decont",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiurnaImpozabila",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "DiurnaImopzabila",
                table: "Decont",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
