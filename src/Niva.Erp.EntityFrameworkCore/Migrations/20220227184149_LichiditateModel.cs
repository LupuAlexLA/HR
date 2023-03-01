using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class LichiditateModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LichidBenzi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Descriere = table.Column<string>(nullable: true),
                    DurataMinima = table.Column<int>(nullable: false),
                    DurataMaxima = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidBenzi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichidConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DenumireRand = table.Column<string>(nullable: true),
                    CodRand = table.Column<string>(nullable: true),
                    EDinConta = table.Column<bool>(nullable: false),
                    FormulaConta = table.Column<string>(nullable: true),
                    FormulaTotal = table.Column<string>(nullable: true),
                    TipInstrument = table.Column<string>(nullable: true),
                    OrderView = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichidCalc",
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
                    LichidBenziId = table.Column<int>(nullable: false),
                    Valoare = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidCalc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichidCalc_LichidBenzi_LichidBenziId",
                        column: x => x.LichidBenziId,
                        principalTable: "LichidBenzi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichidCalc_LichidConfig_LichidConfigId",
                        column: x => x.LichidConfigId,
                        principalTable: "LichidConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LichidCalc_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LichidCalcDet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LichidCalcId = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    Valoare = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichidCalcDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichidCalcDet_LichidCalc_LichidCalcId",
                        column: x => x.LichidCalcId,
                        principalTable: "LichidCalc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalc_LichidBenziId",
                table: "LichidCalc",
                column: "LichidBenziId");

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalc_LichidConfigId",
                table: "LichidCalc",
                column: "LichidConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalc_SavedBalanceId",
                table: "LichidCalc",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LichidCalcDet_LichidCalcId",
                table: "LichidCalcDet",
                column: "LichidCalcId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichidCalcDet");

            migrationBuilder.DropTable(
                name: "LichidCalc");

            migrationBuilder.DropTable(
                name: "LichidBenzi");

            migrationBuilder.DropTable(
                name: "LichidConfig");
        }
    }
}
