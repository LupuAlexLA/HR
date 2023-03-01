using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ImprumutDobanziReferintaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DobanziReferintaId",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_DobanziReferintaId",
                table: "Imprumuturi",
                column: "DobanziReferintaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_DobanziReferinta_DobanziReferintaId",
                table: "Imprumuturi",
                column: "DobanziReferintaId",
                principalTable: "DobanziReferinta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_DobanziReferinta_DobanziReferintaId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_Imprumuturi_DobanziReferintaId",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "DobanziReferintaId",
                table: "Imprumuturi");
        }
    }
}
