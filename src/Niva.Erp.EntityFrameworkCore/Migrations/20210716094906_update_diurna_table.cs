using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_diurna_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dataValabilitate",
                table: "Diurna",
                newName: "DataValabilitate");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Diurna",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Diurna");

            migrationBuilder.RenameColumn(
                name: "DataValabilitate",
                table: "Diurna",
                newName: "dataValabilitate");
        }
    }
}
