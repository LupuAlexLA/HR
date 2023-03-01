using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddAccountHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataValabilitate",
                table: "Account",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AccountHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AccountId = table.Column<int>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    SyntheticAccountId = table.Column<int>(nullable: true),
                    AccountName = table.Column<string>(nullable: false),
                    ExternalCode = table.Column<string>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: true),
                    ThirdPartyId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AccountTypes = table.Column<int>(nullable: false),
                    AccountFuncType = table.Column<int>(nullable: false),
                    ComputingAccount = table.Column<bool>(nullable: false),
                    AccountStatus = table.Column<bool>(nullable: false),
                    TaxStatus = table.Column<int>(nullable: false),
                    BankAccountId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    SectorBnrId = table.Column<int>(nullable: true),
                    NivelRand = table.Column<int>(nullable: true),
                    DataValabilitate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHistory_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHistory_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHistory_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountHistory_BNR_Sector_SectorBnrId",
                        column: x => x.SectorBnrId,
                        principalTable: "BNR_Sector",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountHistory_Account_SyntheticAccountId",
                        column: x => x.SyntheticAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHistory_Persons_ThirdPartyId",
                        column: x => x.ThirdPartyId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_ActivityTypeId",
                table: "AccountHistory",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_BankAccountId",
                table: "AccountHistory",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_CurrencyId",
                table: "AccountHistory",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_SectorBnrId",
                table: "AccountHistory",
                column: "SectorBnrId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_SyntheticAccountId",
                table: "AccountHistory",
                column: "SyntheticAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistory_ThirdPartyId",
                table: "AccountHistory",
                column: "ThirdPartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountHistory");

            migrationBuilder.DropColumn(
                name: "DataValabilitate",
                table: "Account");
        }
    }
}
