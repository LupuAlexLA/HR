using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddTableImoInventariere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImoInventariere",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DataInventariere = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoInventariere", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImoInventariereDet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ImoAssetStockId = table.Column<int>(nullable: true),
                    StockFaptic = table.Column<decimal>(nullable: false),
                    ImoAssetItemId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImoInventariereDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImoInventariereDet_ImoAssetItem_ImoAssetItemId",
                        column: x => x.ImoAssetItemId,
                        principalTable: "ImoAssetItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImoInventariereDet_ImoAssetStock_ImoAssetStockId",
                        column: x => x.ImoAssetStockId,
                        principalTable: "ImoAssetStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImoInventariereDet_ImoAssetItemId",
                table: "ImoInventariereDet",
                column: "ImoAssetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoInventariereDet_ImoAssetStockId",
                table: "ImoInventariereDet",
                column: "ImoAssetStockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImoInventariere");

            migrationBuilder.DropTable(
                name: "ImoInventariereDet");
        }
    }
}
