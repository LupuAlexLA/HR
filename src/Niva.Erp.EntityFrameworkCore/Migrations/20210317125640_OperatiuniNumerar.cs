using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class OperatiuniNumerar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ThirdPartyId = table.Column<int>(nullable: false),
                    ThridPartyId = table.Column<int>(nullable: true),
                    OperationType = table.Column<int>(nullable: false),
                    DispositionNumber = table.Column<int>(nullable: false),
                    DispositionDate = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    DocumentNumber = table.Column<int>(nullable: true),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    InoiceId = table.Column<int>(nullable: true),
                    SumOper = table.Column<decimal>(nullable: false),
                    BankAccountId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositions_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispositions_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispositions_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispositions_Invoices_InoiceId",
                        column: x => x.InoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dispositions_Persons_ThridPartyId",
                        column: x => x.ThridPartyId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_BankAccountId",
                table: "Dispositions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_CurrencyId",
                table: "Dispositions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_DocumentTypeId",
                table: "Dispositions",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_InoiceId",
                table: "Dispositions",
                column: "InoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_ThridPartyId",
                table: "Dispositions",
                column: "ThridPartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dispositions");
        }
    }
}
