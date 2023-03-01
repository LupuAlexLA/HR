using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class PaapTranse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_PAAPTranse",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_PAAPId = table.Column<int>(nullable: false),
                    DataTransa = table.Column<DateTime>(nullable: false),
                    ValoareLei = table.Column<decimal>(nullable: false),
                    ValoareLeiFaraTVA = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_PAAPTranse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_PAAPTranse_BVC_PAAP_BVC_PAAPId",
                        column: x => x.BVC_PAAPId,
                        principalTable: "BVC_PAAP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_PAAPTranse_BVC_PAAPId",
                table: "BVC_PAAPTranse",
                column: "BVC_PAAPId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_PAAPTranse");
        }
    }
}
