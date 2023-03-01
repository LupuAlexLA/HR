using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<int>(
                name: "BankLeiId",
                table: "Exchange",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankValutaId",
                table: "Exchange",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeDate",
                table: "Exchange",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangedValue",
                table: "Exchange",
                nullable: false,
                defaultValue: 0m);

          
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ExchangeDate",
                table: "Exchange");

            migrationBuilder.DropColumn(
                name: "ExchangedValue",
                table: "Exchange");
        }
    }
}
