using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_BVC_DobandaReferinta_Add_Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "BVC_DobandaReferinta",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_DobandaReferinta_CurrencyId",
                table: "BVC_DobandaReferinta",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_DobandaReferinta_Currency_CurrencyId",
                table: "BVC_DobandaReferinta",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_DobandaReferinta_Currency_CurrencyId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropIndex(
                name: "IX_BVC_DobandaReferinta_CurrencyId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "BVC_DobandaReferinta");
        }
    }
}
