using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_BugetPrevContributieTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevContributie",
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
                    BankId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevContributie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevContributie_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevContributie_BankAccount_BankId",
                        column: x => x.BankId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevContributie_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevContributie_ActivityTypeId",
                table: "BVC_BugetPrevContributie",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevContributie_BankId",
                table: "BVC_BugetPrevContributie",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevContributie_CurrencyId",
                table: "BVC_BugetPrevContributie",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BugetPrevContributie");
        }
    }
}
