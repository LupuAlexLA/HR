using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class CurrencyPentruImprumut : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_CurrencyId",
                table: "Imprumuturi",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_Currency_CurrencyId",
                table: "Imprumuturi",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_Currency_CurrencyId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_Imprumuturi_CurrencyId",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Imprumuturi");
        }
    }
}
