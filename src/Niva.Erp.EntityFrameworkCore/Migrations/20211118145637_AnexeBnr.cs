using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AnexeBnr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnexeBnr",
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
                    EDinConta = table.Column<bool>(nullable: false),
                    FormulaConta = table.Column<string>(nullable: true),
                    FormulaTotal = table.Column<string>(nullable: true),
                    TipInstrument = table.Column<string>(nullable: true),
                    DurataMinima = table.Column<int>(nullable: true),
                    DurataMAxima = table.Column<int>(nullable: true),
                    Sectorizare = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnexeBnr", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnexeBnr");
        }
    }
}
