using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_BVC_VenitTitluCFReinv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_VenitTitluCFReinv_ExchangeRateForecasts_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropIndex(
                name: "IX_BVC_VenitTitluCFReinv_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.DropColumn(
                name: "ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.AddColumn<decimal>(
                name: "CursValutarEstimat",
                table: "BVC_VenitTitluCFReinv",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CursValutarEstimat",
                table: "BVC_VenitTitluCFReinv");

            migrationBuilder.AddColumn<int>(
                name: "ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluCFReinv_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv",
                column: "ExchangeRatesForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_VenitTitluCFReinv_ExchangeRateForecasts_ExchangeRatesForecastId",
                table: "BVC_VenitTitluCFReinv",
                column: "ExchangeRatesForecastId",
                principalTable: "ExchangeRateForecasts",
                principalColumn: "Id");
        }
    }
}
