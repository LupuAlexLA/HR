using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class assetStockModerniz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectOperDetail_InvoiceDetails_InvoiceDetailId",
                table: "InvObjectOperDetail");

            migrationBuilder.CreateTable(
                name: "ImoAssetStockModerniz",
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
                    ImoAssetStockId = table.Column<int>(nullable: true),
                    ImoAssetOperDetailId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoAssetStockModerniz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoAssetStockModerniz_ImoAssetOperDetail_ImoAssetOperDetailId",
                        column: x => x.ImoAssetOperDetailId,
                        principalTable: "ImoAssetOperDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoAssetStockModerniz_ImoAssetStock_ImoAssetStockId",
                        column: x => x.ImoAssetStockId,
                        principalTable: "ImoAssetStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStockModerniz_ImoAssetOperDetailId",
                table: "ImoAssetStockModerniz",
                column: "ImoAssetOperDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStockModerniz_ImoAssetStockId",
                table: "ImoAssetStockModerniz",
                column: "ImoAssetStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectOperDetail_InvoiceDetails_InvoiceDetailId",
                table: "InvObjectOperDetail",
                column: "InvoiceDetailId",
                principalTable: "InvoiceDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvObjectOperDetail_InvoiceDetails_InvoiceDetailId",
                table: "InvObjectOperDetail");

            migrationBuilder.DropTable(
                name: "ImoAssetStockModerniz");

            migrationBuilder.AddForeignKey(
                name: "FK_InvObjectOperDetail_InvoiceDetails_InvoiceDetailId",
                table: "InvObjectOperDetail",
                column: "InvoiceDetailId",
                principalTable: "InvoiceDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
