using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BNR_Conturi_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BNR_Conturi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    CodRand = table.Column<string>(nullable: true),
                    Cont = table.Column<string>(nullable: true),
                    ContSintetic = table.Column<string>(nullable: true),
                    SoldDb = table.Column<decimal>(nullable: false),
                    SoldCr = table.Column<decimal>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_Conturi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BNR_Conturi_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BNR_Conturi_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BNR_Conturi_AccountId",
                table: "BNR_Conturi",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_Conturi_SavedBalanceId",
                table: "BNR_Conturi",
                column: "SavedBalanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BNR_Conturi");
        }
    }
}
