using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ExchangeTable_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchange_Persons_BankLeiId",
                table: "Exchange");

            migrationBuilder.DropForeignKey(
                name: "FK_Exchange_Persons_BankValutaId",
                table: "Exchange");

            migrationBuilder.DropIndex(
                name: "IX_Exchange_BankLeiId",
                table: "Exchange");

            migrationBuilder.DropIndex(
                name: "IX_Exchange_BankValutaId",
                table: "Exchange");

            migrationBuilder.DropColumn(
                name: "BankLeiId",
                table: "Exchange");

            migrationBuilder.DropColumn(
                name: "BankValutaId",
                table: "Exchange");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankLeiId",
                table: "Exchange",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankValutaId",
                table: "Exchange",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_BankLeiId",
                table: "Exchange",
                column: "BankLeiId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_BankValutaId",
                table: "Exchange",
                column: "BankValutaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchange_Persons_BankLeiId",
                table: "Exchange",
                column: "BankLeiId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchange_Persons_BankValutaId",
                table: "Exchange",
                column: "BankValutaId",
                principalTable: "Persons",
                principalColumn: "Id");
        }
    }
}
