using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdatePersonTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdBanci",
                table: "Persons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdPersonal",
                table: "Persons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdBanci",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IdPersonal",
                table: "Persons");
        }
    }
}
