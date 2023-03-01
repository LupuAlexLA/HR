using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVCFormularUpd1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormulaTotal",
                table: "BVC_FormRand");

            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "BVC_FormRand",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTotal",
                table: "BVC_FormRand",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formula",
                table: "BVC_FormRand");

            migrationBuilder.DropColumn(
                name: "IsTotal",
                table: "BVC_FormRand");

            migrationBuilder.AddColumn<string>(
                name: "FormulaTotal",
                table: "BVC_FormRand",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
