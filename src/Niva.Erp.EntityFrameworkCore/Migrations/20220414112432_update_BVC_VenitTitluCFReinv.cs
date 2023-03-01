using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_BVC_VenitTitluCFReinv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "BVC_VenitTitluCFReinv",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaIncasata",
                table: "BVC_VenitTitluCFReinv",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluCFReinv_CurrencyId",
                table: "BVC_VenitTitluCFReinv",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluCFReinv_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv",
                column: "ExchangeRatesForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitTitluCFReinv_Currency_CurrencyId",
                table: "BVC_VenitTitluCFReinv",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitTitluCFReinv_ExchangeRateForecasts_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv",
                column: "ExchangeRatesForecastId",
                principalTable: "ExchangeRateForecasts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitTitluCFReinv_Currency_CurrencyId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitTitluCFReinv_ExchangeRateForecasts_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitTitluCFReinv_CurrencyId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitTitluCFReinv_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropColumn(
                name: "ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropColumn(
                name: "SumaIncasata",
                table: "BVC_VenitTitluCFReinv");
        }
    }
}
