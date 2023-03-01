using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddBVCBalRealizat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_BalRealizat",
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
                    table.PrimaryKey("PK_BVC_BalRealizat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BalRealizat_BVC_Formular_BVC_FormularId",
                        column: x => x.BVC_FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_BalRealizat_SavedBalance_SavedBalanceId",
                        column: x => x.SavedBalanceId,
                        principalTable: "SavedBalance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_BalRealizatRand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_BalRealizatId = table.Column<int>(nullable: false),
                    BVC_FormRandId = table.Column<int>(nullable: false),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    Valoare = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BalRealizatRand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BalRealizatRand_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BVC_BalRealizatRand_BVC_BalRealizat_BVC_BalRealizatId",
                        column: x => x.BVC_BalRealizatId,
                        principalTable: "BVC_BalRealizat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_BalRealizatRand_BVC_FormRand_BVC_FormRandId",
                        column: x => x.BVC_FormRandId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_BalRealizatRandDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_BalRealizatRandId = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    Valoare = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_BalRealizatRandDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_BalRealizatRandDetails_BVC_BalRealizatRand_BVC_BalRealizatRandId",
                        column: x => x.BVC_BalRealizatRandId,
                        principalTable: "BVC_BalRealizatRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BalRealizat_BVC_FormularId",
                table: "BVC_BalRealizat",
                column: "BVC_FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BalRealizat_SavedBalanceId",
                table: "BVC_BalRealizat",
                column: "SavedBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BalRealizatRand_ActivityTypeId",
                table: "BVC_BalRealizatRand",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BalRealizatRand_BVC_BalRealizatId",
                table: "BVC_BalRealizatRand",
                column: "BVC_BalRealizatId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BalRealizatRand_BVC_FormRandId",
                table: "BVC_BalRealizatRand",
                column: "BVC_FormRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BalRealizatRandDetails_BVC_BalRealizatRandId",
                table: "BVC_BalRealizatRandDetails",
                column: "BVC_BalRealizatRandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_BalRealizatRandDetails");

            migrationBuilder.DropTable(
                name: "BVC_BalRealizatRand");

            migrationBuilder.DropTable(
                name: "BVC_BalRealizat");
        }
    }
}
