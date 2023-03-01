using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddTable_SavedBalanceViewDet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedBalanceViewDet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(nullable: false),
                    CrValueI = table.Column<decimal>(nullable: false),
                    DbValueI = table.Column<decimal>(nullable: false),
                    CrValueM = table.Column<decimal>(nullable: false),
                    DbValueM = table.Column<decimal>(nullable: false),
                    CrValueY = table.Column<decimal>(nullable: false),
                    DbValueY = table.Column<decimal>(nullable: false),
                    CrValueF = table.Column<decimal>(nullable: false),
                    DbValueF = table.Column<decimal>(nullable: false),
                    DbValueP = table.Column<decimal>(nullable: false),
                    CrValueP = table.Column<decimal>(nullable: false),
                    DbValueT = table.Column<decimal>(nullable: false),
                    CrValueT = table.Column<decimal>(nullable: false),
                    DbValueSum = table.Column<decimal>(nullable: false),
                    CrValueSum = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    NivelRand = table.Column<int>(nullable: true),
                    IsSynthetic = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedBalanceViewDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedBalanceViewDet_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedBalanceViewDet_Balance_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "Balance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedBalanceViewDet_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceViewDet_AccountId",
                table: "SavedBalanceViewDet",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceViewDet_BalanceId",
                table: "SavedBalanceViewDet",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceViewDet_CurrencyId",
                table: "SavedBalanceViewDet",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedBalanceViewDet");
        }
    }
}
