using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ComisionUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrLuni",
                table: "ComisionV2",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareCalculata",
                table: "ComisionV2",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrLuni",
                table: "ComisionV2");

            migrationBuilder.DropColumn(
                name: "ValoareCalculata",
                table: "ComisionV2");
        }
    }
}
