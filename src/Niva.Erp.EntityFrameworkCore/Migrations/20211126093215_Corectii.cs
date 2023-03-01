using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Corectii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TipRandSalarizare",
                table: "BVC_FormRand",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TipRand",
                table: "BVC_FormRand",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "BNR_Raportare",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    AnexaId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_Raportare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BNR_Raportare_BNR_Anexa_AnexaId",
                        column: x => x.AnexaId,
                        principalTable: "BNR_Anexa",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BNR_Raportare_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BNR_RaportareRand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BNR_RaportareId = table.Column<int>(nullable: false),
                    AnexaDetailId = table.Column<int>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    Valoare = table.Column<decimal>(nullable: false),
                    BNR_RaportareId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_RaportareRand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BNR_RaportareRand_BNR_AnexaDetails_AnexaDetailId",
                        column: x => x.AnexaDetailId,
                        principalTable: "BNR_AnexaDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId",
                        column: x => x.BNR_RaportareId,
                        principalTable: "BNR_Raportare",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BNR_RaportareRand_BNR_Raportare_BNR_RaportareId1",
                        column: x => x.BNR_RaportareId1,
                        principalTable: "BNR_Raportare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BNR_RaportareRand_BNR_Sector_SectorId",
                        column: x => x.SectorId,
                        principalTable: "BNR_Sector",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BNR_RaportareRandDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BNR_RaportareRandId = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    Valoare = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BNR_RaportareRandDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BNR_RaportareRandDetail_BNR_RaportareRand_BNR_RaportareRandId",
                        column: x => x.BNR_RaportareRandId,
                        principalTable: "BNR_RaportareRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BNR_Raportare_AnexaId",
                table: "BNR_Raportare",
                column: "AnexaId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_Raportare_SavedBalanceId",
                table: "BNR_Raportare",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_RaportareRand_AnexaDetailId",
                table: "BNR_RaportareRand",
                column: "AnexaDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_RaportareRand_BNR_RaportareId",
                table: "BNR_RaportareRand",
                column: "BNR_RaportareId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_RaportareRand_BNR_RaportareId1",
                table: "BNR_RaportareRand",
                column: "BNR_RaportareId1");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_RaportareRand_SectorId",
                table: "BNR_RaportareRand",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_BNR_RaportareRandDetail_BNR_RaportareRandId",
                table: "BNR_RaportareRandDetail",
                column: "BNR_RaportareRandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BNR_RaportareRandDetail");

            migrationBuilder.DropTable(
                name: "BNR_RaportareRand");

            migrationBuilder.DropTable(
                name: "BNR_Raportare");

            migrationBuilder.AlterColumn<int>(
                name: "TipRandSalarizare",
                table: "BVC_FormRand",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TipRand",
                table: "BVC_FormRand",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
