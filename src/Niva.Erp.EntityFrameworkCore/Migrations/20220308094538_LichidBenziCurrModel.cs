using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class LichidBenziCurrModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichidCalcCurr_Currency_CurrencyId",
                table: "LichidCalcCurr");

            migrationBuilder.DropIndex(
                name: "IX_LichidCalcCurr_CurrencyId",
                table: "LichidCalcCurr");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "LichidCalcCurr");

            migrationBuilder.AddColumn<int>(
                name: "LichidBenziCurrId",
                table: "LichidCalcCurr",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LichidBenziCurr",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Descriere = table.Column<string>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: true),
                    Other = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidBenziCurr", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcCurr_LichidBenziCurrId",
                table: "LichidCalcCurr",
                column: "LichidBenziCurrId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichidCalcCurr_LichidBenziCurr_LichidBenziCurrId",
                table: "LichidCalcCurr",
                column: "LichidBenziCurrId",
                principalTable: "LichidBenziCurr",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichidCalcCurr_LichidBenziCurr_LichidBenziCurrId",
                table: "LichidCalcCurr");

            migrationBuilder.DropTable(
                name: "LichidBenziCurr");

            migrationBuilder.DropIndex(
                name: "IX_LichidCalcCurr_LichidBenziCurrId",
                table: "LichidCalcCurr");

            migrationBuilder.DropColumn(
                name: "LichidBenziCurrId",
                table: "LichidCalcCurr");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "LichidCalcCurr",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcCurr_CurrencyId",
                table: "LichidCalcCurr",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LichidCalcCurr_Currency_CurrencyId",
                table: "LichidCalcCurr",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }
    }
}
