using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddTableInvObjectInventariere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvObjectInventariere",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DataInventariere = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectInventariere", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvObjectInventariereDet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvObjectStockId = table.Column<int>(nullable: true),
                    StockFaptic = table.Column<decimal>(nullable: false),
                    InvObjectItemId = table.Column<int>(nullable: true),
                    InvObjectInventariereId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvObjectInventariereDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvObjectInventariereDet_InvObjectInventariere_InvObjectInventariereId",
                        column: x => x.InvObjectInventariereId,
                        principalTable: "InvObjectInventariere",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvObjectInventariereDet_InvObjectItem_InvObjectItemId",
                        column: x => x.InvObjectItemId,
                        principalTable: "InvObjectItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvObjectInventariereDet_InvObjectStock_InvObjectStockId",
                        column: x => x.InvObjectStockId,
                        principalTable: "InvObjectStock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectInventariereDet_InvObjectInventariereId",
                table: "InvObjectInventariereDet",
                column: "InvObjectInventariereId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectInventariereDet_InvObjectItemId",
                table: "InvObjectInventariereDet",
                column: "InvObjectItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvObjectInventariereDet_InvObjectStockId",
                table: "InvObjectInventariereDet",
                column: "InvObjectStockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvObjectInventariereDet");

            migrationBuilder.DropTable(
                name: "InvObjectInventariere");
        }
    }
}
