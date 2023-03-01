using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DataComision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataComision",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComisionId = table.Column<int>(nullable: true),
                    ImprumutId = table.Column<int>(nullable: true),
                    DataPlataComision = table.Column<DateTime>(nullable: false),
                    SumaComision = table.Column<decimal>(nullable: false),
                    TipValoareComision = table.Column<int>(nullable: false),
                    ValoareComision = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataComision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataComision_Comisioane_ComisionId",
                        column: x => x.ComisionId,
                        principalTable: "Comisioane",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DataComision_Imprumuturi_ImprumutId",
                        column: x => x.ImprumutId,
                        principalTable: "Imprumuturi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataComision_ComisionId",
                table: "DataComision",
                column: "ComisionId");

            migrationBuilder.CreateIndex(
                name: "IX_DataComision_ImprumutId",
                table: "DataComision",
                column: "ImprumutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataComision");
        }
    }
}
