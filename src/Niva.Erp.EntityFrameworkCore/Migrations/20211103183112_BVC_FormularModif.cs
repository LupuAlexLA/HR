using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_FormularModif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.AddColumn<int>(
                name: "BVC_FormularId",
                table: "BVC_FormRand",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BVC_FormRandDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FormRandId = table.Column<int>(nullable: false),
                    TipRandCheltuialaId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    BVC_FormRandId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BVC_FormRandDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BVC_FormRandDetails_BVC_FormRand_BVC_FormRandId",
                        column: x => x.BVC_FormRandId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BVC_FormRandDetails_BVC_FormRand_FormRandId",
                        column: x => x.FormRandId,
                        principalTable: "BVC_FormRand",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BVC_FormRandDetails_InvoiceElementsDetails_TipRandCheltuialaId",
                        column: x => x.TipRandCheltuialaId,
                        principalTable: "InvoiceElementsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRand_BVC_FormularId",
                table: "BVC_FormRand",
                column: "BVC_FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRandDetails_BVC_FormRandId",
                table: "BVC_FormRandDetails",
                column: "BVC_FormRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRandDetails_FormRandId",
                table: "BVC_FormRandDetails",
                column: "FormRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRandDetails_TipRandCheltuialaId",
                table: "BVC_FormRandDetails",
                column: "TipRandCheltuialaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_BVC_FormularId",
                table: "BVC_FormRand",
                column: "BVC_FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_BVC_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropTable(
                name: "BVC_FormRandDetails");

            migrationBuilder.DropIndex(
                name: "IX_BVC_FormRand_BVC_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropColumn(
                name: "BVC_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
