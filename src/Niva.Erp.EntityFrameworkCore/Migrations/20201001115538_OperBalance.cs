using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class OperBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_OperationDetails_ContaOperationDetailId",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Operation_ContaOperationId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Operation_Currency_CurrencyId",
                table: "Operation");

            migrationBuilder.DropForeignKey(
                name: "FK_Operation_DocumentType_DocumentTypeId",
                table: "Operation");

            migrationBuilder.DropForeignKey(
                name: "FK_Operation_OperationDefinition_OperationDefinitionId",
                table: "Operation");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDetails_Account_CreditId",
                table: "OperationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDetails_Account_DebitId",
                table: "OperationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDetails_Operation_OperationId",
                table: "OperationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDetails_Operation_OperationId1",
                table: "OperationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PrepaymentBalance_OperationDetails_OperationDetailId",
                table: "PrepaymentBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationDetails",
                table: "OperationDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operation",
                table: "Operation");

            migrationBuilder.RenameTable(
                name: "OperationDetails",
                newName: "OperationsDetails");

            migrationBuilder.RenameTable(
                name: "Operation",
                newName: "Operations");

            migrationBuilder.RenameIndex(
                name: "IX_OperationDetails_OperationId1",
                table: "OperationsDetails",
                newName: "IX_OperationsDetails_OperationId1");

            migrationBuilder.RenameIndex(
                name: "IX_OperationDetails_OperationId",
                table: "OperationsDetails",
                newName: "IX_OperationsDetails_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationDetails_DebitId",
                table: "OperationsDetails",
                newName: "IX_OperationsDetails_DebitId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationDetails_CreditId",
                table: "OperationsDetails",
                newName: "IX_OperationsDetails_CreditId");

            migrationBuilder.RenameIndex(
                name: "IX_Operation_OperationDefinitionId",
                table: "Operations",
                newName: "IX_Operations_OperationDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_Operation_DocumentTypeId",
                table: "Operations",
                newName: "IX_Operations_DocumentTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Operation_CurrencyId",
                table: "Operations",
                newName: "IX_Operations_CurrencyId");

            migrationBuilder.AddColumn<int>(
                name: "BalanceId",
                table: "SavedBalance",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationsDetails",
                table: "OperationsDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operations",
                table: "Operations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Balance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BalanceDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForeignOperation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BankAccountId = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    DocumentNumber = table.Column<string>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignOperation_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForeignOperation_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForeignOperation_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OrderNr = table.Column<int>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    WrittenValue = table.Column<string>(maxLength: 1000, nullable: true),
                    PayerBankAccountId = table.Column<int>(nullable: false),
                    BeneficiaryId = table.Column<int>(nullable: false),
                    BenefAccountId = table.Column<int>(nullable: false),
                    PaymentDetails = table.Column<string>(maxLength: 1000, nullable: true),
                    InvoiceId = table.Column<int>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    Div = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_BankAccount_BenefAccountId",
                        column: x => x.BenefAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Persons_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_BankAccount_PayerBankAccountId",
                        column: x => x.PayerBankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BalanceDetails",
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
                    CurrencyId = table.Column<int>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceDetails_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BalanceDetails_Balance_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "Balance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BalanceDetails_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForeignOperationsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    DocumentNumber = table.Column<string>(nullable: true),
                    OriginalDetails = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false),
                    ValueCurr = table.Column<decimal>(nullable: false),
                    ForeignOperationId = table.Column<int>(nullable: false),
                    PaymentOrderId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ForeignOperationId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignOperationsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsDetails_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsDetails_ForeignOperation_ForeignOperationId",
                        column: x => x.ForeignOperationId,
                        principalTable: "ForeignOperation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ForeignOperationsDetails_ForeignOperation_ForeignOperationId1",
                        column: x => x.ForeignOperationId1,
                        principalTable: "ForeignOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsDetails_PaymentOrders_PaymentOrderId",
                        column: x => x.PaymentOrderId,
                        principalTable: "PaymentOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForeignOperationsAccounting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false),
                    ValueCurr = table.Column<decimal>(nullable: false),
                    DebitId = table.Column<int>(nullable: true),
                    CreditId = table.Column<int>(nullable: true),
                    FgnOperationsDetailId = table.Column<int>(nullable: false),
                    OperationsDetailId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignOperationsAccounting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsAccounting_Account_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsAccounting_Account_DebitId",
                        column: x => x.DebitId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsAccounting_ForeignOperationsDetails_FgnOperationsDetailId",
                        column: x => x.FgnOperationsDetailId,
                        principalTable: "ForeignOperationsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForeignOperationsAccounting_OperationsDetails_OperationsDetailId",
                        column: x => x.OperationsDetailId,
                        principalTable: "OperationsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedBalance_BalanceId",
                table: "SavedBalance",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceDetails_AccountId",
                table: "BalanceDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceDetails_BalanceId",
                table: "BalanceDetails",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceDetails_CurrencyId",
                table: "BalanceDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperation_BankAccountId",
                table: "ForeignOperation",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperation_CurrencyId",
                table: "ForeignOperation",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperation_DocumentTypeId",
                table: "ForeignOperation",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsAccounting_CreditId",
                table: "ForeignOperationsAccounting",
                column: "CreditId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsAccounting_DebitId",
                table: "ForeignOperationsAccounting",
                column: "DebitId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsAccounting_FgnOperationsDetailId",
                table: "ForeignOperationsAccounting",
                column: "FgnOperationsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsAccounting_OperationsDetailId",
                table: "ForeignOperationsAccounting",
                column: "OperationsDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsDetails_DocumentTypeId",
                table: "ForeignOperationsDetails",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsDetails_ForeignOperationId",
                table: "ForeignOperationsDetails",
                column: "ForeignOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsDetails_ForeignOperationId1",
                table: "ForeignOperationsDetails",
                column: "ForeignOperationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignOperationsDetails_PaymentOrderId",
                table: "ForeignOperationsDetails",
                column: "PaymentOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_BenefAccountId",
                table: "PaymentOrders",
                column: "BenefAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_BeneficiaryId",
                table: "PaymentOrders",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_CurrencyId",
                table: "PaymentOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_InvoiceId",
                table: "PaymentOrders",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_PayerBankAccountId",
                table: "PaymentOrders",
                column: "PayerBankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_OperationsDetails_ContaOperationDetailId",
                table: "InvoiceDetails",
                column: "ContaOperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Operations_ContaOperationId",
                table: "Invoices",
                column: "ContaOperationId",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Currency_CurrencyId",
                table: "Operations",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_DocumentType_DocumentTypeId",
                table: "Operations",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_OperationDefinition_OperationDefinitionId",
                table: "Operations",
                column: "OperationDefinitionId",
                principalTable: "OperationDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Account_CreditId",
                table: "OperationsDetails",
                column: "CreditId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Account_DebitId",
                table: "OperationsDetails",
                column: "DebitId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails",
                column: "OperationId",
                principalTable: "Operations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId1",
                table: "OperationsDetails",
                column: "OperationId1",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrepaymentBalance_OperationsDetails_OperationDetailId",
                table: "PrepaymentBalance",
                column: "OperationDetailId",
                principalTable: "OperationsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedBalance_Balance_BalanceId",
                table: "SavedBalance",
                column: "BalanceId",
                principalTable: "Balance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_OperationsDetails_ContaOperationDetailId",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Operations_ContaOperationId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Currency_CurrencyId",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_DocumentType_DocumentTypeId",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_Operations_OperationDefinition_OperationDefinitionId",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Account_CreditId",
                table: "OperationsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Account_DebitId",
                table: "OperationsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId",
                table: "OperationsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId1",
                table: "OperationsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PrepaymentBalance_OperationsDetails_OperationDetailId",
                table: "PrepaymentBalance");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedBalance_Balance_BalanceId",
                table: "SavedBalance");

            migrationBuilder.DropTable(
                name: "BalanceDetails");

            migrationBuilder.DropTable(
                name: "ForeignOperationsAccounting");

            migrationBuilder.DropTable(
                name: "Balance");

            migrationBuilder.DropTable(
                name: "ForeignOperationsDetails");

            migrationBuilder.DropTable(
                name: "ForeignOperation");

            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_SavedBalance_BalanceId",
                table: "SavedBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationsDetails",
                table: "OperationsDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operations",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "BalanceId",
                table: "SavedBalance");

            migrationBuilder.RenameTable(
                name: "OperationsDetails",
                newName: "OperationDetails");

            migrationBuilder.RenameTable(
                name: "Operations",
                newName: "Operation");

            migrationBuilder.RenameIndex(
                name: "IX_OperationsDetails_OperationId1",
                table: "OperationDetails",
                newName: "IX_OperationDetails_OperationId1");

            migrationBuilder.RenameIndex(
                name: "IX_OperationsDetails_OperationId",
                table: "OperationDetails",
                newName: "IX_OperationDetails_OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationsDetails_DebitId",
                table: "OperationDetails",
                newName: "IX_OperationDetails_DebitId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationsDetails_CreditId",
                table: "OperationDetails",
                newName: "IX_OperationDetails_CreditId");

            migrationBuilder.RenameIndex(
                name: "IX_Operations_OperationDefinitionId",
                table: "Operation",
                newName: "IX_Operation_OperationDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_Operations_DocumentTypeId",
                table: "Operation",
                newName: "IX_Operation_DocumentTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Operations_CurrencyId",
                table: "Operation",
                newName: "IX_Operation_CurrencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationDetails",
                table: "OperationDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operation",
                table: "Operation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_OperationDetails_ContaOperationDetailId",
                table: "InvoiceDetails",
                column: "ContaOperationDetailId",
                principalTable: "OperationDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Operation_ContaOperationId",
                table: "Invoices",
                column: "ContaOperationId",
                principalTable: "Operation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Operation_Currency_CurrencyId",
                table: "Operation",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operation_DocumentType_DocumentTypeId",
                table: "Operation",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operation_OperationDefinition_OperationDefinitionId",
                table: "Operation",
                column: "OperationDefinitionId",
                principalTable: "OperationDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDetails_Account_CreditId",
                table: "OperationDetails",
                column: "CreditId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDetails_Account_DebitId",
                table: "OperationDetails",
                column: "DebitId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDetails_Operation_OperationId",
                table: "OperationDetails",
                column: "OperationId",
                principalTable: "Operation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDetails_Operation_OperationId1",
                table: "OperationDetails",
                column: "OperationId1",
                principalTable: "Operation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrepaymentBalance_OperationDetails_OperationDetailId",
                table: "PrepaymentBalance",
                column: "OperationDetailId",
                principalTable: "OperationDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
