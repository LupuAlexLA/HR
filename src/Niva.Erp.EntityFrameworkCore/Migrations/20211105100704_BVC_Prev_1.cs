using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_Prev_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BugetPrevRandStatus");

            migrationBuilder.AddColumn<bool>(
                name: "Validat",
                table: "BVC_BugetPrevRand",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BugetPrevId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    BVC_BugetPrevId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BVC_BugetPrevId",
                        column: x => x.BVC_BugetPrevId,
                        principalTable: "BVC_BugetPrev",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                        column: x => x.BugetPrevId,
                        principalTable: "BVC_BugetPrev",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevStatus_BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BVC_BugetPrevId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevStatus_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BugetPrevId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BugetPrevStatus");

            migrationBuilder.DropColumn(
                name: "Validat",
                table: "BVC_BugetPrevRand");

            migrationBuilder.CreateTable(
                name: "BVC_BugetPrevRandStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BVC_BugetPrevRandId = table.Column<int>(type: "int", nullable: true),
                    BugetPrevRandId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BugetPrevRandStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandStatus_BVC_BugetPrevRand_BVC_BugetPrevRandId",
                        column: x => x.BVC_BugetPrevRandId,
                        principalTable: "BVC_BugetPrevRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BVC_BugetPrevRandStatus_BVC_BugetPrevRand_BugetPrevRandId",
                        column: x => x.BugetPrevRandId,
                        principalTable: "BVC_BugetPrevRand",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandStatus_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandStatus",
                column: "BVC_BugetPrevRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandStatus_BugetPrevRandId",
                table: "BVC_BugetPrevRandStatus",
                column: "BugetPrevRandId");
        }
    }
}
