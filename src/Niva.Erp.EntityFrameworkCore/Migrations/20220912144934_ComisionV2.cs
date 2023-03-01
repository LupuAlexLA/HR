using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ComisionV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComisionV2",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImprumutId = table.Column<int>(nullable: true),
                    TipComision = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    TipValoareComision = table.Column<int>(nullable: false),
                    ValoareComision = table.Column<decimal>(nullable: false),
                    ModCalculComision = table.Column<int>(nullable: false),
                    TipSumaComision = table.Column<int>(nullable: false),
                    BazaDeCalcul = table.Column<int>(nullable: false),
                    DataStart = table.Column<DateTime>(nullable: false),
                    DataEnd = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComisionV2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComisionV2_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperatieDobandaComision",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImprumutId = table.Column<int>(nullable: true),
                    OperGenerateId = table.Column<int>(nullable: true),
                    OperationId = table.Column<int>(nullable: true),
                    TipOperatieDobandaComision = table.Column<int>(nullable: false),
                    DataStart = table.Column<DateTime>(nullable: false),
                    DataEnd = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatieDobandaComision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatieDobandaComision_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperatieDobandaComision_OperGenerate_OperGenerateId",
                        column: x => x.OperGenerateId,
                        principalTable: "OperGenerate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperatieDobandaComision_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComisionV2_ImprumutId",
                table: "ComisionV2",
                column: "ImprumutId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatieDobandaComision_ImprumutId",
                table: "OperatieDobandaComision",
                column: "ImprumutId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatieDobandaComision_OperGenerateId",
                table: "OperatieDobandaComision",
                column: "OperGenerateId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatieDobandaComision_OperationId",
                table: "OperatieDobandaComision",
                column: "OperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComisionV2");

            migrationBuilder.DropTable(
                name: "OperatieDobandaComision");
        }
    }
}
