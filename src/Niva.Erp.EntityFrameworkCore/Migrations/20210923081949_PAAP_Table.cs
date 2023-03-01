using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class PAAP_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paap",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SursaFinantare",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SursaFinantare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PAAP_Stari",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DateStare = table.Column<DateTime>(nullable: false),
                    StatePAAP = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    PAAPId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAAP_Stari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PAAP_Stari_Paap_PAAPId",
                        column: x => x.PAAPId,
                        principalTable: "Paap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BVC_PAAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PaapId = table.Column<int>(nullable: false),
                    ObiectTranzactie = table.Column<string>(nullable: true),
                    CodCPV = table.Column<string>(nullable: true),
                    ValoareEstimataFaraTvaLei = table.Column<decimal>(nullable: false),
                    SursaFinantareId = table.Column<int>(nullable: false),
                    DataStart = table.Column<DateTime>(nullable: false),
                    DataEnd = table.Column<DateTime>(nullable: false),
                    ModalitateDerulare = table.Column<int>(nullable: false),
                    InvoiceElementsDetailsId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PAAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_InvoiceElementsDetails_InvoiceElementsDetailsId",
                        column: x => x.InvoiceElementsDetailsId,
                        principalTable: "InvoiceElementsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_Paap_PaapId",
                        column: x => x.PaapId,
                        principalTable: "Paap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_SursaFinantare_SursaFinantareId",
                        column: x => x.SursaFinantareId,
                        principalTable: "SursaFinantare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_Structs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    StructNumber = table.Column<string>(nullable: true),
                    OrderView = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    BVC_PAAPId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Structs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_Structs_BVC_PAAP_BVC_PAAPId",
                        column: x => x.BVC_PAAPId,
                        principalTable: "BVC_PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_Planificare",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_Year = table.Column<DateTime>(nullable: false),
                    BVC_Date = table.Column<DateTime>(nullable: false),
                    Department = table.Column<int>(nullable: false),
                    BVC_StructId = table.Column<int>(nullable: false),
                    PaymentDate = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Planificare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_Planificare_BVC_Structs_BVC_StructId",
                        column: x => x.BVC_StructId,
                        principalTable: "BVC_Structs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_InvoiceElementsDetailsId",
                table: "BVC_PAAP",
                column: "InvoiceElementsDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_PaapId",
                table: "BVC_PAAP",
                column: "PaapId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_SursaFinantareId",
                table: "BVC_PAAP",
                column: "SursaFinantareId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Planificare_BVC_StructId",
                table: "BVC_Planificare",
                column: "BVC_StructId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Structs_BVC_PAAPId",
                table: "BVC_Structs",
                column: "BVC_PAAPId");

            migrationBuilder.CreateIndex(
                name: "IX_PAAP_Stari_PAAPId",
                table: "PAAP_Stari",
                column: "PAAPId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_Planificare");

            migrationBuilder.DropTable(
                name: "PAAP_Stari");

            migrationBuilder.DropTable(
                name: "BVC_Structs");

            migrationBuilder.DropTable(
                name: "BVC_PAAP");

            migrationBuilder.DropTable(
                name: "Paap");

            migrationBuilder.DropTable(
                name: "SursaFinantare");
        }
    }
}
