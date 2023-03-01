using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class CupiuriTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CupiuriItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CupiuriItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CupiuriItem_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CupiuriDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CupiuriItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CupiuriDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CupiuriDetails_CupiuriItem_CupiuriItemId",
                        column: x => x.CupiuriItemId,
                        principalTable: "CupiuriItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CupiuriDetails_CupiuriItemId",
                table: "CupiuriDetails",
                column: "CupiuriItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CupiuriItem_CurrencyId",
                table: "CupiuriItem",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CupiuriDetails");

            migrationBuilder.DropTable(
                name: "CupiuriItem");
        }
    }
}
