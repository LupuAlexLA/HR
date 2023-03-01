using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class _3rdpartyetc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ThirdParties_ThirdPartyId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_Bank_BankId",
                table: "BankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_ThirdParties_ThirdPartyId",
                table: "BankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_ThirdParties_ThirdPartyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_ThirdParties_ThirdPartyId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Prepayment_ThirdParties_ThirdPartyId",
                table: "Prepayment");

            migrationBuilder.DropTable(
                name: "Bank");

            migrationBuilder.DropTable(
                name: "ThirdParties");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_ThirdPartyId",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "ThirdPartyId",
                table: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "BankAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Issuer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LegalPersonId = table.Column<int>(nullable: false),
                    IbanAbrv = table.Column<string>(maxLength: 1000, nullable: true),
                    Bic = table.Column<string>(maxLength: 1000, nullable: true),
                    IssuerType = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issuer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issuer_Persons_LegalPersonId",
                        column: x => x.LegalPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_PersonId",
                table: "BankAccount",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Issuer_LegalPersonId",
                table: "Issuer",
                column: "LegalPersonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Persons_ThirdPartyId",
                table: "Account",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_Issuer_BankId",
                table: "BankAccount",
                column: "BankId",
                principalTable: "Issuer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_Persons_PersonId",
                table: "BankAccount",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Persons_ThirdPartyId",
                table: "Contracts",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Persons_ThirdPartyId",
                table: "Invoices",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prepayment_Persons_ThirdPartyId",
                table: "Prepayment",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Persons_ThirdPartyId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_Issuer_BankId",
                table: "BankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_Persons_PersonId",
                table: "BankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Persons_ThirdPartyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Persons_ThirdPartyId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Prepayment_Persons_ThirdPartyId",
                table: "Prepayment");

            migrationBuilder.DropTable(
                name: "Issuer");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_PersonId",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "ThirdPartyId",
                table: "BankAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bic = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    IbanAbrv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    LegalPersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bank_Persons_LegalPersonId",
                        column: x => x.LegalPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThirdParties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsClient = table.Column<bool>(type: "bit", nullable: false),
                    IsOther = table.Column<bool>(type: "bit", nullable: false),
                    IsProvider = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThirdParties_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_ThirdPartyId",
                table: "BankAccount",
                column: "ThirdPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Bank_LegalPersonId",
                table: "Bank",
                column: "LegalPersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThirdParties_PersonId",
                table: "ThirdParties",
                column: "PersonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ThirdParties_ThirdPartyId",
                table: "Account",
                column: "ThirdPartyId",
                principalTable: "ThirdParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_Bank_BankId",
                table: "BankAccount",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_ThirdParties_ThirdPartyId",
                table: "BankAccount",
                column: "ThirdPartyId",
                principalTable: "ThirdParties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_ThirdParties_ThirdPartyId",
                table: "Contracts",
                column: "ThirdPartyId",
                principalTable: "ThirdParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_ThirdParties_ThirdPartyId",
                table: "Invoices",
                column: "ThirdPartyId",
                principalTable: "ThirdParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prepayment_ThirdParties_ThirdPartyId",
                table: "Prepayment",
                column: "ThirdPartyId",
                principalTable: "ThirdParties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
