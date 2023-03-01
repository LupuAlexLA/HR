using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableImoInventariere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImoInventariereId",
                table: "ImoInventariereDet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImoInventariereDet_ImoInventariereId",
                table: "ImoInventariereDet",
                column: "ImoInventariereId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoInventariereDet_ImoInventariere_ImoInventariereId",
                table: "ImoInventariereDet",
                column: "ImoInventariereId",
                principalTable: "ImoInventariere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoInventariereDet_ImoInventariere_ImoInventariereId",
                table: "ImoInventariereDet");

            migrationBuilder.DropIndex(
                name: "IX_ImoInventariereDet_ImoInventariereId",
                table: "ImoInventariereDet");

            migrationBuilder.DropColumn(
                name: "ImoInventariereId",
                table: "ImoInventariereDet");
        }
    }
}
