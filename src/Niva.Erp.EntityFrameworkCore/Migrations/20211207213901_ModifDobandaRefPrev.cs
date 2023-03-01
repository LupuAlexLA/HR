using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ModifDobandaRefPrev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "An",
                table: "BVC_DobandaReferinta");

            migrationBuilder.AlterColumn<int>(
                name: "PlasamentType",
                table: "BVC_DobandaReferinta",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEnd",
                table: "BVC_DobandaReferinta",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataStart",
                table: "BVC_DobandaReferinta",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FormularId",
                table: "BVC_DobandaReferinta",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_DobandaReferinta_FormularId",
                table: "BVC_DobandaReferinta",
                column: "FormularId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_DobandaReferinta_BVC_Formular_FormularId",
                table: "BVC_DobandaReferinta",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_DobandaReferinta_BVC_Formular_FormularId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropIndex(
                name: "IX_BVC_DobandaReferinta_FormularId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropColumn(
                name: "DataEnd",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropColumn(
                name: "DataStart",
                table: "BVC_DobandaReferinta");

            migrationBuilder.DropColumn(
                name: "FormularId",
                table: "BVC_DobandaReferinta");

            migrationBuilder.AlterColumn<int>(
                name: "PlasamentType",
                table: "BVC_DobandaReferinta",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "An",
                table: "BVC_DobandaReferinta",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
