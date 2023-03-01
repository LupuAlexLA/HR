using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddInvObjectStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvObjectStock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvObjectItemId = table.Column<int>(nullable: false),
                    OperType = table.Column<int>(nullable: false),
                    StockDate = table.Column<DateTime>(nullable: false),
                    TranzQuantity = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TranzDuration = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    StorageId = table.Column<int>(nullable: false),
                    TranzInventoryValue = table.Column<decimal>(nullable: false),
                    InventoryValue = table.Column<decimal>(nullable: false),
                    TranzFiscalInventoryValue = table.Column<decimal>(nullable: false),
                    FiscalInventoryValue = table.Column<decimal>(nullable: false),
                    TranzDeprec = table.Column<decimal>(nullable: false),
                    Deprec = table.Column<decimal>(nullable: false),
                    TranzFiscalDeprec = table.Column<decimal>(nullable: false),
                    FiscalDeprec = table.Column<decimal>(nullable: false),
                    InConservare = table.Column<bool>(nullable: false),
                    MonthlyDepreciation = table.Column<decimal>(nullable: false),
                    MonthlyFiscalDepreciation = table.Column<decimal>(nullable: false),
                    InvObjectItemPFId = table.Column<int>(nullable: true),
                    InvObjectOperDetId = table.Column<int>(nullable: true),
                    InvObjectAccounttId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectStock_Account_InvObjectAccounttId",
                        column: x => x.InvObjectAccounttId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectStock_InvObjectItem_InvObjectItemId",
                        column: x => x.InvObjectItemId,
                        principalTable: "InvObjectItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvObjectStock_InvObjectItem_InvObjectItemPFId",
                        column: x => x.InvObjectItemPFId,
                        principalTable: "InvObjectItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectStock_InvObjectOperDetail_InvObjectOperDetId",
                        column: x => x.InvObjectOperDetId,
                        principalTable: "InvObjectOperDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectStock_InvStorage_StorageId",
                        column: x => x.StorageId,
                        principalTable: "InvStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvObjectStockReserve",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TranzDeprecReserve = table.Column<decimal>(nullable: false),
                    DeprecReserve = table.Column<decimal>(nullable: false),
                    TranzReserve = table.Column<decimal>(nullable: false),
                    Reserve = table.Column<decimal>(nullable: false),
                    ExpenseReserve = table.Column<decimal>(nullable: false),
                    InvObjectStockId = table.Column<int>(nullable: true),
                    InvObjectOperDetailId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectStockReserve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectStockReserve_InvObjectOperDetail_InvObjectOperDetailId",
                        column: x => x.InvObjectOperDetailId,
                        principalTable: "InvObjectOperDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectStockReserve_InvObjectStock_InvObjectStockId",
                        column: x => x.InvObjectStockId,
                        principalTable: "InvObjectStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStock_InvObjectAccounttId",
                table: "InvObjectStock",
                column: "InvObjectAccounttId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStock_InvObjectItemId",
                table: "InvObjectStock",
                column: "InvObjectItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStock_InvObjectItemPFId",
                table: "InvObjectStock",
                column: "InvObjectItemPFId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStock_InvObjectOperDetId",
                table: "InvObjectStock",
                column: "InvObjectOperDetId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStock_StorageId",
                table: "InvObjectStock",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStockReserve_InvObjectOperDetailId",
                table: "InvObjectStockReserve",
                column: "InvObjectOperDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectStockReserve_InvObjectStockId",
                table: "InvObjectStockReserve",
                column: "InvObjectStockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvObjectStockReserve");

            migrationBuilder.DropTable(
                name: "InvObjectStock");
        }
    }
}
