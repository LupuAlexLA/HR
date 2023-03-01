using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class VenitCheltZileLibere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenitType",
                table: "BVC_VenitTitlu",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BVC_VenitCheltuieli",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_VenitTitluId = table.Column<int>(nullable: false),
                    BVC_BugetPrevRandValueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitCheltuieli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitCheltuieli_BVC_BugetPrevRandValue_BVC_BugetPrevRandValueId",
                        column: x => x.BVC_BugetPrevRandValueId,
                        principalTable: "BVC_BugetPrevRandValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_VenitCheltuieli_BVC_VenitTitlu_BVC_VenitTitluId",
                        column: x => x.BVC_VenitTitluId,
                        principalTable: "BVC_VenitTitlu",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ZileLibere",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ZiLibera = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZileLibere", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitCheltuieli_BVC_BugetPrevRandValueId",
                table: "BVC_VenitCheltuieli",
                column: "BVC_BugetPrevRandValueId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitCheltuieli_BVC_VenitTitluId",
                table: "BVC_VenitCheltuieli",
                column: "BVC_VenitTitluId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_VenitCheltuieli");

            migrationBuilder.DropTable(
                name: "ZileLibere");

            migrationBuilder.DropColumn(
                name: "VenitType",
                table: "BVC_VenitTitlu");
        }
    }
}
