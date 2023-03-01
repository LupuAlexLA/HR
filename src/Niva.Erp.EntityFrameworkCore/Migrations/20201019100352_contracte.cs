using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class contracte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceDetails_Account_AccountId",
                table: "BalanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractsCategory_ContractsCategoryId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Account_CreditId",
                table: "OperationsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Operations_OperationId1",
                table: "OperationsDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationsDetails_OperationId1",
                table: "OperationsDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractsCategory",
                table: "ContractsCategory");

            migrationBuilder.DropColumn(
                name: "OperationId1",
                table: "OperationsDetails");

            migrationBuilder.RenameTable(
                name: "ContractsCategory",
                newName: "ContractsCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractsCategories",
                table: "ContractsCategories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InvoiceElementAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvoiceElementAccountType = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceElementAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceElementAccounts_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceElementAccounts_AccountId",
                table: "InvoiceElementAccounts",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceDetails_Account_AccountId",
                table: "BalanceDetails",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractsCategories_ContractsCategoryId",
                table: "Contracts",
                column: "ContractsCategoryId",
                principalTable: "ContractsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationsDetails_Account_CreditId",
                table: "OperationsDetails",
                column: "CreditId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceDetails_Account_AccountId",
                table: "BalanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ContractsCategories_ContractsCategoryId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationsDetails_Account_CreditId",
                table: "OperationsDetails");

            migrationBuilder.DropTable(
                name: "InvoiceElementAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractsCategories",
                table: "ContractsCategories");

            migrationBuilder.RenameTable(
                name: "ContractsCategories",
                newName: "ContractsCategory");

            migrationBuilder.AddColumn<int>(
                name: "OperationId1",
                table: "OperationsDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractsCategory",
                table: "ContractsCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OperationsDetails_OperationId1",
                table: "OperationsDetails",
                column: "OperationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceDetails_Account_AccountId",
                table: "BalanceDetails",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ContractsCategory_ContractsCategoryId",
                table: "Contracts",
                column: "ContractsCategoryId",
                principalTable: "ContractsCategory",
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
                name: "FK_OperationsDetails_Operations_OperationId1",
                table: "OperationsDetails",
                column: "OperationId1",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
