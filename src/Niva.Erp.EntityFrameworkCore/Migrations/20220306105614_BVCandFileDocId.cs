using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVCandFileDocId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileDocId",
                table: "Decont",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FileDocId",
                table: "Contracts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BalFormulaBVC",
                table: "BVC_FormRand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BalFormulaCashFlow",
                table: "BVC_FormRand",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BalIsTotal",
                table: "BVC_FormRand",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileDocId",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "FileDocId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "BalFormulaBVC",
                table: "BVC_FormRand");

            migrationBuilder.DropColumn(
                name: "BalFormulaCashFlow",
                table: "BVC_FormRand");

            migrationBuilder.DropColumn(
                name: "BalIsTotal",
                table: "BVC_FormRand");
        }
    }
}
