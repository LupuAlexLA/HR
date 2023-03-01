using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BugetSumeReinvestite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevSumeReinvest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BugetPrevId = table.Column<int>(nullable: false),
                    IdPlasament = table.Column<string>(nullable: true),
                    TipPlasament = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    ValoarePlasament = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    MaturityDate = table.Column<DateTime>(nullable: false),
                    VenitType = table.Column<int>(nullable: false),
                    ProcentDobanda = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevSumeReinvest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevSumeReinvest_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevSumeReinvest_BVC_BugetPrev_BugetPrevId",
                        column: x => x.BugetPrevId,
                        principalTable: "BVC_BugetPrev",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevSumeReinvest_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevSumeReinvest_ActivityTypeId",
                table: "BVC_BugetPrevSumeReinvest",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevSumeReinvest_BugetPrevId",
                table: "BVC_BugetPrevSumeReinvest",
                column: "BugetPrevId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevSumeReinvest_CurrencyId",
                table: "BVC_BugetPrevSumeReinvest",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BugetPrevSumeReinvest");
        }
    }
}
