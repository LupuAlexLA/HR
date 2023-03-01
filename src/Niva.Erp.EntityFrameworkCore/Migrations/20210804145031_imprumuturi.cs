using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class imprumuturi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImprumutTermene",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    MinValue = table.Column<int>(nullable: false),
                    MaxValue = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImprumutTermene", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImprumutTipuri",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    Account = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImprumutTipuri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Imprumuturi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    DocumentNr = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    ImprumuturiTipuriId = table.Column<int>(nullable: false),
                    LoanAccountId = table.Column<int>(nullable: false),
                    PaymentAccountId = table.Column<int>(nullable: false),
                    LoanDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Durata = table.Column<int>(nullable: false),
                    ImprumuturiTipDurata = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ImprumuturiTermenId = table.Column<int>(nullable: false),
                    Suma = table.Column<decimal>(nullable: false),
                    ProcentDobanda = table.Column<decimal>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imprumuturi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imprumuturi_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Imprumuturi_ImprumutTermene_ImprumuturiTermenId",
                        column: x => x.ImprumuturiTermenId,
                        principalTable: "ImprumutTermene",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Imprumuturi_ImprumutTipuri_ImprumuturiTipuriId",
                        column: x => x.ImprumuturiTipuriId,
                        principalTable: "ImprumutTipuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Imprumuturi_BankAccount_LoanAccountId",
                        column: x => x.LoanAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imprumuturi_BankAccount_PaymentAccountId",
                        column: x => x.PaymentAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_DocumentTypeId",
                table: "Imprumuturi",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_ImprumuturiTermenId",
                table: "Imprumuturi",
                column: "ImprumuturiTermenId");

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_ImprumuturiTipuriId",
                table: "Imprumuturi",
                column: "ImprumuturiTipuriId");

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_LoanAccountId",
                table: "Imprumuturi",
                column: "LoanAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_PaymentAccountId",
                table: "Imprumuturi",
                column: "PaymentAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imprumuturi");

            migrationBuilder.DropTable(
                name: "ImprumutTermene");

            migrationBuilder.DropTable(
                name: "ImprumutTipuri");
        }
    }
}
