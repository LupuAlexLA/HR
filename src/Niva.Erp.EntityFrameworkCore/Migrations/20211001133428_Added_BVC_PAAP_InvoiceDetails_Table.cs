using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Added_BVC_PAAP_InvoiceDetails_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_PAAP_InvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    InvoiceDetailsId = table.Column<int>(nullable: false),
                    BVC_PAAPId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PAAP_InvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_InvoiceDetails_BVC_PAAP_BVC_PAAPId",
                        column: x => x.BVC_PAAPId,
                        principalTable: "BVC_PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_PAAP_InvoiceDetails_InvoiceDetails_InvoiceDetailsId",
                        column: x => x.InvoiceDetailsId,
                        principalTable: "InvoiceDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_InvoiceDetails_BVC_PAAPId",
                table: "BVC_PAAP_InvoiceDetails",
                column: "BVC_PAAPId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_InvoiceDetails_InvoiceDetailsId",
                table: "BVC_PAAP_InvoiceDetails",
                column: "InvoiceDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_PAAP_InvoiceDetails");
        }
    }
}
