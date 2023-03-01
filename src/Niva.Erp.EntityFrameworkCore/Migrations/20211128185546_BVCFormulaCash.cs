using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVCFormulaCash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formula",
                table: "BVC_FormRand");

            migrationBuilder.AddColumn<string>(
                name: "FormulaBVC",
                table: "BVC_FormRand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaCashFlow",
                table: "BVC_FormRand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormulaBVC",
                table: "BVC_FormRand");

            migrationBuilder.DropColumn(
                name: "FormulaCashFlow",
                table: "BVC_FormRand");

            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "BVC_FormRand",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
