using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class modificareBVC_PAAP_State : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_State_Currency_CurrencyId",
                table: "BVC_PAAP_State");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_State_CurrencyId",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "Comentarii",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "DataEnd",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "TVA",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "ValoareEstimataFaraTVA",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "ValoareaTotala",
                table: "BVC_PAAP_State");

            migrationBuilder.DropColumn(
                name: "ValoareaTotalaValuta",
                table: "BVC_PAAP_State");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comentarii",
                table: "BVC_PAAP_State",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "BVC_PAAP_State",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEnd",
                table: "BVC_PAAP_State",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TVA",
                table: "BVC_PAAP_State",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareEstimataFaraTVA",
                table: "BVC_PAAP_State",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareaTotala",
                table: "BVC_PAAP_State",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValoareaTotalaValuta",
                table: "BVC_PAAP_State",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_State_CurrencyId",
                table: "BVC_PAAP_State",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_State_Currency_CurrencyId",
                table: "BVC_PAAP_State",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
