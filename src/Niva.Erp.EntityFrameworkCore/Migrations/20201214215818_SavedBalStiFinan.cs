using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class SavedBalStiFinan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SitFinanRapId",
                table: "SitFinanCalc",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SitFinanCalcFormulaDet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    SitFinanRapRowId = table.Column<int>(nullable: false),
                    ColumnId = table.Column<int>(nullable: false),
                    Formula = table.Column<string>(maxLength: 1000, nullable: true),
                    FormulaVal = table.Column<string>(maxLength: 1000, nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitFinanCalcFormulaDet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitFinanCalcFormulaDet_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SitFinanCalcFormulaDet_SitFinanRapConfig_SitFinanRapRowId",
                        column: x => x.SitFinanRapRowId,
                        principalTable: "SitFinanRapConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitFinanCalcNote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SavedBalanceId = table.Column<int>(nullable: false),
                    SitFinanRapId = table.Column<int>(nullable: false),
                    BeforeNote = table.Column<string>(nullable: true),
                    AfterNote = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitFinanCalcNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitFinanCalcNote_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SitFinanCalcNote_SitFinanRap_SitFinanRapId",
                        column: x => x.SitFinanRapId,
                        principalTable: "SitFinanRap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitFinanRapConfigCol",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ColumnNr = table.Column<int>(nullable: false),
                    ColumnName = table.Column<string>(maxLength: 1000, nullable: true),
                    ColumnModCalc = table.Column<int>(nullable: true),
                    SitFinanRapId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitFinanRapConfigCol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitFinanRapConfigCol_SitFinanRap_SitFinanRapId",
                        column: x => x.SitFinanRapId,
                        principalTable: "SitFinanRap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitFinanRapConfigCorel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OrderView = table.Column<decimal>(nullable: false),
                    DescribeCol1 = table.Column<string>(maxLength: 1000, nullable: true),
                    FormulaCol1 = table.Column<string>(maxLength: 1000, nullable: true),
                    DescribeCol2 = table.Column<string>(maxLength: 1000, nullable: true),
                    FormulaCol2 = table.Column<string>(maxLength: 1000, nullable: true),
                    SitFinanRapId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitFinanRapConfigCorel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitFinanRapConfigCorel_SitFinanRap_SitFinanRapId",
                        column: x => x.SitFinanRapId,
                        principalTable: "SitFinanRap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SitFinanRapConfigNote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BeforeNote = table.Column<string>(nullable: true),
                    AfterNote = table.Column<string>(nullable: true),
                    SitFinanRapId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitFinanRapConfigNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitFinanRapConfigNote_SitFinanRap_SitFinanRapId",
                        column: x => x.SitFinanRapId,
                        principalTable: "SitFinanRap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanCalc_SitFinanRapId",
                table: "SitFinanCalc",
                column: "SitFinanRapId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanCalcFormulaDet_SavedBalanceId",
                table: "SitFinanCalcFormulaDet",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanCalcFormulaDet_SitFinanRapRowId",
                table: "SitFinanCalcFormulaDet",
                column: "SitFinanRapRowId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanCalcNote_SavedBalanceId",
                table: "SitFinanCalcNote",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanCalcNote_SitFinanRapId",
                table: "SitFinanCalcNote",
                column: "SitFinanRapId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanRapConfigCol_SitFinanRapId",
                table: "SitFinanRapConfigCol",
                column: "SitFinanRapId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanRapConfigCorel_SitFinanRapId",
                table: "SitFinanRapConfigCorel",
                column: "SitFinanRapId");

            migrationBuilder.CreateIndex(
                name: "IX_SitFinanRapConfigNote_SitFinanRapId",
                table: "SitFinanRapConfigNote",
                column: "SitFinanRapId");

            migrationBuilder.AddForeignKey(
                name: "FK_SitFinanCalc_SitFinanRap_SitFinanRapId",
                table: "SitFinanCalc",
                column: "SitFinanRapId",
                principalTable: "SitFinanRap",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SitFinanCalc_SitFinanRap_SitFinanRapId",
                table: "SitFinanCalc");

            migrationBuilder.DropTable(
                name: "SitFinanCalcFormulaDet");

            migrationBuilder.DropTable(
                name: "SitFinanCalcNote");

            migrationBuilder.DropTable(
                name: "SitFinanRapConfigCol");

            migrationBuilder.DropTable(
                name: "SitFinanRapConfigCorel");

            migrationBuilder.DropTable(
                name: "SitFinanRapConfigNote");

            migrationBuilder.DropIndex(
                name: "IX_SitFinanCalc_SitFinanRapId",
                table: "SitFinanCalc");

            migrationBuilder.DropColumn(
                name: "SitFinanRapId",
                table: "SitFinanCalc");
        }
    }
}
