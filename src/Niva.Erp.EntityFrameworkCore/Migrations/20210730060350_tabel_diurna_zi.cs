using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class tabel_diurna_zi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diurna");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiurnaAcordata",
                table: "Decont",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiurnaImpozabila",
                table: "Decont",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "DiurnaLegala",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    DataValabilitate = table.Column<DateTime>(nullable: false),
                    DiurnaType = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiurnaLegala", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiurnaLegala_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiurnaLegala_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiurnaZi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    DataValabilitate = table.Column<DateTime>(nullable: false),
                    DiurnaType = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiurnaZi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiurnaZi_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiurnaZi_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiurnaLegala_CountryId",
                table: "DiurnaLegala",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiurnaLegala_CurrencyId",
                table: "DiurnaLegala",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_DiurnaZi_CountryId",
                table: "DiurnaZi",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiurnaZi_CurrencyId",
                table: "DiurnaZi",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiurnaLegala");

            migrationBuilder.DropTable(
                name: "DiurnaZi");

            migrationBuilder.DropColumn(
                name: "TotalDiurnaAcordata",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "TotalDiurnaImpozabila",
                table: "Decont");

            migrationBuilder.CreateTable(
                name: "Diurna",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    DataValabilitate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diurna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diurna_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Diurna_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diurna_CountryId",
                table: "Diurna",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Diurna_CurrencyId",
                table: "Diurna",
                column: "CurrencyId");
        }
    }
}
