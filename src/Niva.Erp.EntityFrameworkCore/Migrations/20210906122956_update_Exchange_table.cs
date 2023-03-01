using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_Exchange_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchange_ExchangeRates_ExchangeRateId",
                table: "Exchange");

            migrationBuilder.DropIndex(
                name: "IX_Exchange_ExchangeRateId",
                table: "Exchange");

            migrationBuilder.DropColumn(
                name: "ExchangeRateId",
                table: "Exchange");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Exchange",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Exchange");

            migrationBuilder.AddColumn<int>(
                name: "ExchangeRateId",
                table: "Exchange",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_ExchangeRateId",
                table: "Exchange",
                column: "ExchangeRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchange_ExchangeRates_ExchangeRateId",
                table: "Exchange",
                column: "ExchangeRateId",
                principalTable: "ExchangeRates",
                principalColumn: "Id");
        }
    }
}
