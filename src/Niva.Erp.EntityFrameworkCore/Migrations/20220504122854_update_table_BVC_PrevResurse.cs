using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_BVC_PrevResurse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BugetPrevId",
                table: "BVC_PrevResurse",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PrevResurse_BugetPrevId",
                table: "BVC_PrevResurse",
                column: "BugetPrevId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PrevResurse_BVC_BugetPrev_BugetPrevId",
                table: "BVC_PrevResurse",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PrevResurse_BVC_BugetPrev_BugetPrevId",
                table: "BVC_PrevResurse");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PrevResurse_BugetPrevId",
                table: "BVC_PrevResurse");

            migrationBuilder.DropColumn(
                name: "BugetPrevId",
                table: "BVC_PrevResurse");
        }
    }
}
