using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BvcVenituri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_VenitTitlu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FormularId = table.Column<int>(nullable: false),
                    IdPlasament = table.Column<string>(nullable: true),
                    TipPlasament = table.Column<string>(nullable: true),
                    ActivityTypeId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    ValoarePlasament = table.Column<decimal>(nullable: false),
                    MaturityDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitTitlu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitlu_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitlu_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitlu_BVC_Formular_FormularId",
                        column: x => x.FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_VenitTitluBVC",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_VenitTitluId = table.Column<int>(nullable: false),
                    DataDobanda = table.Column<DateTime>(nullable: false),
                    ValoarePlasament = table.Column<decimal>(nullable: false),
                    DobandaLuna = table.Column<decimal>(nullable: false),
                    DobandaCumulataPrec = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitTitluBVC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitluBVC_BVC_VenitTitlu_BVC_VenitTitluId",
                        column: x => x.BVC_VenitTitluId,
                        principalTable: "BVC_VenitTitlu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BVC_VenitTitluCF",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BVC_VenitTitluId = table.Column<int>(nullable: false),
                    DataIncasare = table.Column<DateTime>(nullable: false),
                    ValoarePlasament = table.Column<decimal>(nullable: false),
                    DobandaTotala = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_VenitTitluCF", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_VenitTitluCF_BVC_VenitTitlu_BVC_VenitTitluId",
                        column: x => x.BVC_VenitTitluId,
                        principalTable: "BVC_VenitTitlu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitlu_ActivityTypeId",
                table: "BVC_VenitTitlu",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitlu_CurrencyId",
                table: "BVC_VenitTitlu",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitlu_FormularId",
                table: "BVC_VenitTitlu",
                column: "FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluBVC_BVC_VenitTitluId",
                table: "BVC_VenitTitluBVC",
                column: "BVC_VenitTitluId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_VenitTitluCF_BVC_VenitTitluId",
                table: "BVC_VenitTitluCF",
                column: "BVC_VenitTitluId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_VenitTitluBVC");

            migrationBuilder.DropTable(
                name: "BVC_VenitTitluCF");

            migrationBuilder.DropTable(
                name: "BVC_VenitTitlu");
        }
    }
}
