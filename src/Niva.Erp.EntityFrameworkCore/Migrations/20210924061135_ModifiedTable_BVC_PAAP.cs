using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ModifiedTable_BVC_PAAP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_PAAP_PAAPId",
                table: "BVC_PAAP");

            migrationBuilder.DropTable(
                name: "PAAP_Stari");

            migrationBuilder.DropTable(
                name: "PAAP");

            migrationBuilder.DropIndex(
                name: "IX_BVC_PAAP_PAAPId",
                table: "BVC_PAAP");

            migrationBuilder.DropColumn(
                name: "PAAPId",
                table: "BVC_PAAP");

            migrationBuilder.AddColumn<int>(
                name: "PaapState",
                table: "BVC_PAAP",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaapState",
                table: "BVC_PAAP");

            migrationBuilder.AddColumn<int>(
                name: "PAAPId",
                table: "BVC_PAAP",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PAAP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAAP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PAAP_Stari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DateStare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    PAAPId = table.Column<int>(type: "int", nullable: true),
                    StatePAAP = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAAP_Stari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PAAP_Stari_PAAP_PAAPId",
                        column: x => x.PAAPId,
                        principalTable: "PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAP_PAAPId",
                table: "BVC_PAAP",
                column: "PAAPId");

            migrationBuilder.CreateIndex(
                name: "IX_PAAP_Stari_PAAPId",
                table: "PAAP_Stari",
                column: "PAAPId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_PAAP_PAAPId",
                table: "BVC_PAAP",
                column: "PAAPId",
                principalTable: "PAAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
