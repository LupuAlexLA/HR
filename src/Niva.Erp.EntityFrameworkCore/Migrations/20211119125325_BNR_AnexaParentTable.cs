using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BNR_AnexaParentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DobandaReferintaType",
                table: "BVC_DobandaReferinta");

            migrationBuilder.AddColumn<int>(
                name: "PlasamentType",
                table: "BVC_DobandaReferinta",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnexaParentId",
                table: "AnexeBnr",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BNR_AnexaParent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Denumire = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_AnexaParent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnexeBnr_AnexaParentId",
                table: "AnexeBnr",
                column: "AnexaParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnexeBnr_BNR_AnexaParent_AnexaParentId",
                table: "AnexeBnr",
                column: "AnexaParentId",
                principalTable: "BNR_AnexaParent",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnexeBnr_BNR_AnexaParent_AnexaParentId",
                table: "AnexeBnr");

            migrationBuilder.DropTable(
                name: "BNR_AnexaParent");

            migrationBuilder.DropIndex(
                name: "IX_AnexeBnr_AnexaParentId",
                table: "AnexeBnr");

            migrationBuilder.DropColumn(
                name: "PlasamentType",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropColumn(
                name: "AnexaParentId",
                table: "AnexeBnr");

            migrationBuilder.AddColumn<int>(
                name: "DobandaReferintaType",
                table: "BVC_DobandaReferinta",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
