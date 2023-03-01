using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class addTable_SavedBalanceDetailsCurrencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedBalanceDetailsCurrencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(nullable: false),
                    CrValueF = table.Column<decimal>(nullable: false),
                    CrValueI = table.Column<decimal>(nullable: false),
                    CrValueM = table.Column<decimal>(nullable: false),
                    CrValueY = table.Column<decimal>(nullable: false),
                    DbValueF = table.Column<decimal>(nullable: false),
                    DbValueI = table.Column<decimal>(nullable: false),
                    DbValueM = table.Column<decimal>(nullable: false),
                    DbValueY = table.Column<decimal>(nullable: false),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedBalanceDetailsCurrencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedBalanceDetailsCurrencies_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedBalanceDetailsCurrencies_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedBalanceDetailsCurrencies_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceDetailsCurrencies_AccountId",
                table: "SavedBalanceDetailsCurrencies",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceDetailsCurrencies_CurrencyId",
                table: "SavedBalanceDetailsCurrencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalanceDetailsCurrencies_SavedBalanceId",
                table: "SavedBalanceDetailsCurrencies",
                column: "SavedBalanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedBalanceDetailsCurrencies");
        }
    }
}
