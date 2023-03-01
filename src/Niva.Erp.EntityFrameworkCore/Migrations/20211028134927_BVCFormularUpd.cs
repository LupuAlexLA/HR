using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVCFormularUpd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BVC_Formular",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AnBVC = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_Formular", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BVC_FormRand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FormularId = table.Column<int>(nullable: false),
                    CodRand = table.Column<int>(nullable: false),
                    Descriere = table.Column<string>(nullable: true),
                    TipRand = table.Column<int>(nullable: false),
                    OrderView = table.Column<int>(nullable: false),
                    RandParentId = table.Column<int>(nullable: true),
                    FormulaTotal = table.Column<string>(nullable: true),
                    TipRandVenit = table.Column<int>(nullable: false),
                    TipRandCheltuialaId = table.Column<int>(nullable: true),
                    AvailableBVC = table.Column<bool>(nullable: false),
                    AvailableCashFlow = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_FormRand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                        column: x => x.FormularId,
                        principalTable: "BVC_Formular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BVC_FormRand_BVC_FormRand_RandParentId",
                        column: x => x.RandParentId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BVC_FormRand_InvoiceElementsDetails_TipRandCheltuialaId",
                        column: x => x.TipRandCheltuialaId,
                        principalTable: "InvoiceElementsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRand_FormularId",
                table: "BVC_FormRand",
                column: "FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRand_RandParentId",
                table: "BVC_FormRand",
                column: "RandParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRand_TipRandCheltuialaId",
                table: "BVC_FormRand",
                column: "TipRandCheltuialaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BVC_FormRand");

            migrationBuilder.DropTable(
                name: "BVC_Formular");
        }
    }
}
