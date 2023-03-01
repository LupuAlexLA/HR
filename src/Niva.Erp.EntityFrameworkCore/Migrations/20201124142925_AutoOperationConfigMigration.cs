using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AutoOperationConfigMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoOperationConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AutoOperType = table.Column<int>(nullable: false),
                    OperationType = table.Column<int>(nullable: false),
                    ValueSign = table.Column<int>(nullable: false),
                    ElementId = table.Column<int>(nullable: false),
                    DebitAccount = table.Column<string>(maxLength: 1000, nullable: true),
                    CreditAccount = table.Column<string>(maxLength: 1000, nullable: true),
                    Details = table.Column<string>(maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    EntryOrder = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    IndividualOperation = table.Column<bool>(nullable: false),
                    UnreceiveInvoice = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoOperationConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoOperationOper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AutoOperType = table.Column<int>(nullable: false),
                    OperationType = table.Column<int>(nullable: false),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    DocumentNumber = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Validated = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoOperationOper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoOperationOper_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutoOperationOper_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutoOperationDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AutoOperId = table.Column<int>(nullable: false),
                    DebitAccountId = table.Column<int>(nullable: false),
                    CreditAccountId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    ValueCurr = table.Column<decimal>(nullable: false),
                    Details = table.Column<string>(nullable: true),
                    OperationalId = table.Column<int>(nullable: false),
                    OperationDetailId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoOperationDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoOperationDetail_AutoOperationOper_AutoOperId",
                        column: x => x.AutoOperId,
                        principalTable: "AutoOperationOper",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutoOperationDetail_Account_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AutoOperationDetail_Account_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AutoOperationDetail_OperationsDetails_OperationDetailId",
                        column: x => x.OperationDetailId,
                        principalTable: "OperationsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationDetail_AutoOperId",
                table: "AutoOperationDetail",
                column: "AutoOperId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationDetail_CreditAccountId",
                table: "AutoOperationDetail",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationDetail_DebitAccountId",
                table: "AutoOperationDetail",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationDetail_OperationDetailId",
                table: "AutoOperationDetail",
                column: "OperationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationOper_CurrencyId",
                table: "AutoOperationOper",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoOperationOper_DocumentTypeId",
                table: "AutoOperationOper",
                column: "DocumentTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoOperationConfig");

            migrationBuilder.DropTable(
                name: "AutoOperationDetail");

            migrationBuilder.DropTable(
                name: "AutoOperationOper");
        }
    }
}
