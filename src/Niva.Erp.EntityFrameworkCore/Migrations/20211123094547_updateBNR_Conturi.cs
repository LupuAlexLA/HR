using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateBNR_Conturi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnexeBnr");

            migrationBuilder.DropTable(
                name: "BNR_AnexaParent");

            migrationBuilder.DropColumn(
                name: "CodRand",
                table: "BNR_Conturi");

            migrationBuilder.DropColumn(
                name: "Cont",
                table: "BNR_Conturi");

            migrationBuilder.DropColumn(
                name: "ContSintetic",
                table: "BNR_Conturi");

            migrationBuilder.DropColumn(
                name: "State",
                table: "BNR_Conturi");

            migrationBuilder.AddColumn<int>(
                name: "AnexaDetailId",
                table: "BNR_Conturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BNR_SectorId",
                table: "BNR_Conturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContBNR",
                table: "BNR_Conturi",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BNR_Anexa",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Denumire = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_Anexa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BNR_AnexaDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    NrCrt = table.Column<string>(nullable: true),
                    DenumireRand = table.Column<string>(nullable: true),
                    CodRand = table.Column<string>(nullable: true),
                    EDinConta = table.Column<bool>(nullable: false),
                    FormulaConta = table.Column<string>(nullable: true),
                    FormulaTotal = table.Column<string>(nullable: true),
                    TipInstrument = table.Column<string>(nullable: true),
                    DurataMinima = table.Column<int>(nullable: true),
                    DurataMaxima = table.Column<int>(nullable: true),
                    Sectorizare = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    AnexaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_AnexaDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BNR_AnexaDetails_BNR_Anexa_AnexaId",
                        column: x => x.AnexaId,
                        principalTable: "BNR_Anexa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BNR_Conturi_AnexaDetailId",
                table: "BNR_Conturi",
                column: "AnexaDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_Conturi_BNR_SectorId",
                table: "BNR_Conturi",
                column: "BNR_SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_AnexaDetails_AnexaId",
                table: "BNR_AnexaDetails",
                column: "AnexaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BNR_Conturi_BNR_AnexaDetails_AnexaDetailId",
                table: "BNR_Conturi",
                column: "AnexaDetailId",
                principalTable: "BNR_AnexaDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BNR_Conturi_BNR_Sector_BNR_SectorId",
                table: "BNR_Conturi",
                column: "BNR_SectorId",
                principalTable: "BNR_Sector",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BNR_Conturi_BNR_AnexaDetails_AnexaDetailId",
                table: "BNR_Conturi");

            migrationBuilder.DropForeignKey(
                name: "FK_BNR_Conturi_BNR_Sector_BNR_SectorId",
                table: "BNR_Conturi");

            migrationBuilder.DropTable(
                name: "BNR_AnexaDetails");

            migrationBuilder.DropTable(
                name: "BNR_Anexa");

            migrationBuilder.DropIndex(
                name: "IX_BNR_Conturi_AnexaDetailId",
                table: "BNR_Conturi");

            migrationBuilder.DropIndex(
                name: "IX_BNR_Conturi_BNR_SectorId",
                table: "BNR_Conturi");

            migrationBuilder.DropColumn(
                name: "AnexaDetailId",
                table: "BNR_Conturi");

            migrationBuilder.DropColumn(
                name: "BNR_SectorId",
                table: "BNR_Conturi");

            migrationBuilder.DropColumn(
                name: "ContBNR",
                table: "BNR_Conturi");

            migrationBuilder.AddColumn<string>(
                name: "CodRand",
                table: "BNR_Conturi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cont",
                table: "BNR_Conturi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContSintetic",
                table: "BNR_Conturi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "BNR_Conturi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BNR_AnexaParent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Denumire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_AnexaParent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnexeBnr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnexaParentId = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DenumireRand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurataMAxima = table.Column<int>(type: "int", nullable: true),
                    DurataMinima = table.Column<int>(type: "int", nullable: true),
                    EDinConta = table.Column<bool>(type: "bit", nullable: false),
                    FormulaConta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormulaTotal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    NrCrt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sectorizare = table.Column<bool>(type: "bit", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    TipInstrument = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnexeBnr", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnexeBnr_BNR_AnexaParent_AnexaParentId",
                        column: x => x.AnexaParentId,
                        principalTable: "BNR_AnexaParent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnexeBnr_AnexaParentId",
                table: "AnexeBnr",
                column: "AnexaParentId");
        }
    }
}
