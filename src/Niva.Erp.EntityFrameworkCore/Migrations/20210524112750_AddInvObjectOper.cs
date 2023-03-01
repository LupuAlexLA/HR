using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddInvObjectOper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvObjectOper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    DocumentNr = table.Column<int>(nullable: false),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    InvObjectsOperType = table.Column<int>(nullable: false),
                    InvObjectsStoreInId = table.Column<int>(nullable: true),
                    InvObjectsStoreOutId = table.Column<int>(nullable: true),
                    Processed = table.Column<bool>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectOper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectOper_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvObjectOper_InvStorage_InvObjectsStoreInId",
                        column: x => x.InvObjectsStoreInId,
                        principalTable: "InvStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectOper_InvStorage_InvObjectsStoreOutId",
                        column: x => x.InvObjectsStoreOutId,
                        principalTable: "InvStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectOper_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvObjectOperDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvObjectItemId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    InvValueModif = table.Column<decimal>(nullable: false),
                    InvObjectOperId = table.Column<int>(nullable: false),
                    InvoiceDetailId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectOperDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectOperDetail_InvObjectItem_InvObjectItemId",
                        column: x => x.InvObjectItemId,
                        principalTable: "InvObjectItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvObjectOperDetail_InvObjectOper_InvObjectOperId",
                        column: x => x.InvObjectOperId,
                        principalTable: "InvObjectOper",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvObjectOperDetail_InvoiceDetails_InvoiceDetailId",
                        column: x => x.InvoiceDetailId,
                        principalTable: "InvoiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOper_DocumentTypeId",
                table: "InvObjectOper",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOper_InvObjectsStoreInId",
                table: "InvObjectOper",
                column: "InvObjectsStoreInId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOper_InvObjectsStoreOutId",
                table: "InvObjectOper",
                column: "InvObjectsStoreOutId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOper_InvoiceId",
                table: "InvObjectOper",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOperDetail_InvObjectItemId",
                table: "InvObjectOperDetail",
                column: "InvObjectItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOperDetail_InvObjectOperId",
                table: "InvObjectOperDetail",
                column: "InvObjectOperId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectOperDetail_InvoiceDetailId",
                table: "InvObjectOperDetail",
                column: "InvoiceDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvObjectOperDetail");

            migrationBuilder.DropTable(
                name: "InvObjectOper");
        }
    }
}
