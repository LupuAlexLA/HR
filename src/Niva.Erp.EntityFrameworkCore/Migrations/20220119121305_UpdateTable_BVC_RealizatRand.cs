using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTable_BVC_RealizatRand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Valoare",
                table: "BVC_RealizatRand");

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareCuReferat",
                table: "BVC_RealizatRand",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareFaraReferat",
                table: "BVC_RealizatRand",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValoareCuReferat",
                table: "BVC_RealizatRand");

            migrationBuilder.DropColumn(
                name: "ValoareFaraReferat",
                table: "BVC_RealizatRand");

            migrationBuilder.AddColumn<decimal>(
                name: "Valoare",
                table: "BVC_RealizatRand",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
