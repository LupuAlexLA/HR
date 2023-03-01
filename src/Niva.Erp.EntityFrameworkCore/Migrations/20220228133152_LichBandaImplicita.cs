using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class LichBandaImplicita : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LichidBenziId",
                table: "LichidConfig",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichidConfig_LichidBenziId",
                table: "LichidConfig",
                column: "LichidBenziId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichidConfig_LichidBenzi_LichidBenziId",
                table: "LichidConfig",
                column: "LichidBenziId",
                principalTable: "LichidBenzi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichidConfig_LichidBenzi_LichidBenziId",
                table: "LichidConfig");

            migrationBuilder.DropIndex(
                name: "IX_LichidConfig_LichidBenziId",
                table: "LichidConfig");

            migrationBuilder.DropColumn(
                name: "LichidBenziId",
                table: "LichidConfig");
        }
    }
}
