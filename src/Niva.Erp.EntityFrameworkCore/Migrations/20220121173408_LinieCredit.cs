using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class LinieCredit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tragere",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataTragere = table.Column<DateTime>(nullable: false),
                    SumaDisponibila = table.Column<decimal>(nullable: false),
                    SumaTrasa = table.Column<decimal>(nullable: false),
                    Dobanda = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ImprumutId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tragere", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tragere_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tragere_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tragere_CurrencyId",
                table: "Tragere",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tragere_ImprumutId",
                table: "Tragere",
                column: "ImprumutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tragere");
        }
    }
}
