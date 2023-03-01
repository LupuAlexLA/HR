using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class modificareBVC_PAAP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comentarii",
                table: "BVC_PAAP");

            migrationBuilder.AddColumn<string>(
                name: "Comentarii",
                table: "BVC_PAAP_State",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comentarii",
                table: "BVC_PAAP_State");

            migrationBuilder.AddColumn<string>(
                name: "Comentarii",
                table: "BVC_PAAP",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
