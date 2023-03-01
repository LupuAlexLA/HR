using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_Cheltuieli2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_Cheltuieli",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DataIncasare = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    BVC_FormRandId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Cheltuieli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_Cheltuieli_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_Cheltuieli_BVC_FormRand_BVC_FormRandId",
                        column: x => x.BVC_FormRandId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_Cheltuieli_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Cheltuieli_ActivityTypeId",
                table: "BVC_Cheltuieli",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Cheltuieli_BVC_FormRandId",
                table: "BVC_Cheltuieli",
                column: "BVC_FormRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Cheltuieli_CurrencyId",
                table: "BVC_Cheltuieli",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_Cheltuieli");
        }
    }
}
