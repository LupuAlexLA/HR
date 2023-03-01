using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class LichCurrCalcModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LichidCalcCurr",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    LichidConfigId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Valoare = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidCalcCurr", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichidCalcCurr_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichidCalcCurr_LichidConfig_LichidConfigId",
                        column: x => x.LichidConfigId,
                        principalTable: "LichidConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichidCalcCurr_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LichidCalcCurrDet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LichidCalcCurrId = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    Valoare = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidCalcCurrDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichidCalcCurrDet_LichidCalcCurr_LichidCalcCurrId",
                        column: x => x.LichidCalcCurrId,
                        principalTable: "LichidCalcCurr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcCurr_CurrencyId",
                table: "LichidCalcCurr",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcCurr_LichidConfigId",
                table: "LichidCalcCurr",
                column: "LichidConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcCurr_SavedBalanceId",
                table: "LichidCalcCurr",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcCurrDet_LichidCalcCurrId",
                table: "LichidCalcCurrDet",
                column: "LichidCalcCurrId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichidCalcCurrDet");

            migrationBuilder.DropTable(
                name: "LichidCalcCurr");
        }
    }
}
