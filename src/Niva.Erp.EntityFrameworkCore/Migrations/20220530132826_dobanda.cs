using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class dobanda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dobanda",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RataId = table.Column<int>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    ValoareDobanda = table.Column<decimal>(nullable: false),
                    ValoarePrincipal = table.Column<decimal>(nullable: false),
                    ContaOperationId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dobanda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dobanda_Operations_ContaOperationId",
                        column: x => x.ContaOperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dobanda_Rate_RataId",
                        column: x => x.RataId,
                        principalTable: "Rate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DobandaEveniment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DobandaId = table.Column<int>(nullable: true),
                    ValoareDobandaDatorata = table.Column<decimal>(nullable: false),
                    ValoarePrincipalDatorat = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DobandaEveniment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DobandaEveniment_Dobanda_DobandaId",
                        column: x => x.DobandaId,
                        principalTable: "Dobanda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dobanda_ContaOperationId",
                table: "Dobanda",
                column: "ContaOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Dobanda_RataId",
                table: "Dobanda",
                column: "RataId");

            migrationBuilder.CreateIndex(
                name: "IX_DobandaEveniment_DobandaId",
                table: "DobandaEveniment",
                column: "DobandaId",
                unique: true,
                filter: "[DobandaId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DobandaEveniment");

            migrationBuilder.DropTable(
                name: "Dobanda");
        }
    }
}
