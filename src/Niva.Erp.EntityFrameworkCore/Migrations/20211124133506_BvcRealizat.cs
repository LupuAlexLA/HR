using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BvcRealizat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_Realizat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    BVC_FormularId = table.Column<int>(nullable: false),
                    BVC_Tip = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Realizat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_Realizat_BVC_Formular_BVC_FormularId",
                        column: x => x.BVC_FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_Realizat_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_RealizatRand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_RealizatId = table.Column<int>(nullable: false),
                    BVC_FormRandId = table.Column<int>(nullable: false),
                    Valoare = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_RealizatRand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_RealizatRand_BVC_FormRand_BVC_FormRandId",
                        column: x => x.BVC_FormRandId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_RealizatRand_BVC_Realizat_BVC_RealizatId",
                        column: x => x.BVC_RealizatId,
                        principalTable: "BVC_Realizat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_RealizatRandDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_RealizatRandId = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    Valoare = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_RealizatRandDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_RealizatRandDetails_BVC_RealizatRand_BVC_RealizatRandId",
                        column: x => x.BVC_RealizatRandId,
                        principalTable: "BVC_RealizatRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Realizat_BVC_FormularId",
                table: "BVC_Realizat",
                column: "BVC_FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_Realizat_SavedBalanceId",
                table: "BVC_Realizat",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_RealizatRand_BVC_FormRandId",
                table: "BVC_RealizatRand",
                column: "BVC_FormRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_RealizatRand_BVC_RealizatId",
                table: "BVC_RealizatRand",
                column: "BVC_RealizatId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_RealizatRandDetails_BVC_RealizatRandId",
                table: "BVC_RealizatRandDetails",
                column: "BVC_RealizatRandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_RealizatRandDetails");

            migrationBuilder.DropTable(
                name: "BVC_RealizatRand");

            migrationBuilder.DropTable(
                name: "BVC_Realizat");
        }
    }
}
