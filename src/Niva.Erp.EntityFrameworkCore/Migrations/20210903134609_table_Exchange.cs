using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class table_Exchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchange",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ExchangeOperType = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    BankAccountLeiId = table.Column<int>(nullable: false),
                    BankAccountValutaId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    ExchangeRateId = table.Column<int>(nullable: false),
                    ExchangeType = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exchange_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exchange_BankAccount_BankAccountLeiId",
                        column: x => x.BankAccountLeiId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exchange_BankAccount_BankAccountValutaId",
                        column: x => x.BankAccountValutaId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exchange_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exchange_ExchangeRates_ExchangeRateId",
                        column: x => x.ExchangeRateId,
                        principalTable: "ExchangeRates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_ActivityTypeId",
                table: "Exchange",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_BankAccountLeiId",
                table: "Exchange",
                column: "BankAccountLeiId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_BankAccountValutaId",
                table: "Exchange",
                column: "BankAccountValutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_CurrencyId",
                table: "Exchange",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchange_ExchangeRateId",
                table: "Exchange",
                column: "ExchangeRateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exchange");
        }
    }
}
