using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class currencyPentruRateSiGarantie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Rate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rate_CurrencyId",
                table: "Rate",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_CurrencyId",
                table: "Garantie",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_Currency_CurrencyId",
                table: "Garantie",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_Currency_CurrencyId",
                table: "Rate",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_Currency_CurrencyId",
                table: "Garantie");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_Currency_CurrencyId",
                table: "Rate");

            migrationBuilder.DropIndex(
                name: "IX_Rate_CurrencyId",
                table: "Rate");

            migrationBuilder.DropIndex(
                name: "IX_Garantie_CurrencyId",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Garantie");
        }
    }
}
