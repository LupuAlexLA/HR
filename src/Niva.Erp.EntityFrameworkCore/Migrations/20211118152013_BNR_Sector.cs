using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BNR_Sector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BNR_SectorId",
                table: "Issuer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BNR_Sector",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Sector = table.Column<string>(nullable: true),
                    Denumire = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_Sector", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issuer_BNR_SectorId",
                table: "Issuer",
                column: "BNR_SectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issuer_BNR_Sector_BNR_SectorId",
                table: "Issuer",
                column: "BNR_SectorId",
                principalTable: "BNR_Sector",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issuer_BNR_Sector_BNR_SectorId",
                table: "Issuer");

            migrationBuilder.DropTable(
                name: "BNR_Sector");

            migrationBuilder.DropIndex(
                name: "IX_Issuer_BNR_SectorId",
                table: "Issuer");

            migrationBuilder.DropColumn(
                name: "BNR_SectorId",
                table: "Issuer");
        }
    }
}
