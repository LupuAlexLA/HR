using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Addgarantii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Garantie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartenerGarantie = table.Column<string>(nullable: true),
                    TipGarantie = table.Column<string>(nullable: true),
                    Document = table.Column<string>(nullable: true),
                    GarantieAccountId = table.Column<int>(nullable: true),
                    SumaGarantiei = table.Column<decimal>(nullable: false),
                    Mentiuni = table.Column<string>(nullable: true),
                    CeGaranteaza = table.Column<string>(nullable: true),
                    StartDateGarantie = table.Column<DateTime>(nullable: false),
                    EndDateGarantie = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    ImprumutId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garantie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garantie_BankAccount_GarantieAccountId",
                        column: x => x.GarantieAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Garantie_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_GarantieAccountId",
                table: "Garantie",
                column: "GarantieAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Garantie_ImprumutId",
                table: "Garantie",
                column: "ImprumutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Garantie");
        }
    }
}
