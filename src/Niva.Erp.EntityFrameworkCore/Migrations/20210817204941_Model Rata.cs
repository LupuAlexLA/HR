using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ModelRata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerioadaTipDurata",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Periodicitate",
                table: "Imprumuturi",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Rate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumarOrdinDePlata = table.Column<int>(nullable: false),
                    DataPlataRata = table.Column<DateTime>(nullable: false),
                    SumaPrincipal = table.Column<decimal>(nullable: false),
                    SumaDobanda = table.Column<decimal>(nullable: false),
                    SumaPlatita = table.Column<decimal>(nullable: false),
                    Sold = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ImprumutId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rate_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rate_ImprumutId",
                table: "Rate",
                column: "ImprumutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rate");

            migrationBuilder.DropColumn(
                name: "PerioadaTipDurata",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "Periodicitate",
                table: "Imprumuturi");
        }
    }
}
