using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_Prev_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BVC_BugetPrevId",
                table: "BVC_BugetPrevRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_BVC_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropIndex(
                name: "IX_BVC_FormRand_BVC_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropIndex(
                name: "IX_BVC_BugetPrevStatus_BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.DropIndex(
                name: "IX_BVC_BugetPrevRandValue_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropIndex(
                name: "IX_BVC_BugetPrevRand_BVC_BugetPrevId",
                table: "BVC_BugetPrevRand");

            migrationBuilder.DropColumn(
                name: "BVC_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropColumn(
                name: "BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.DropColumn(
                name: "BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropColumn(
                name: "BVC_BugetPrevId",
                table: "BVC_BugetPrevRand");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BVC_FormularId",
                table: "BVC_FormRand",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BVC_BugetPrevId",
                table: "BVC_BugetPrevRand",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRand_BVC_FormularId",
                table: "BVC_FormRand",
                column: "BVC_FormularId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevStatus_BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BVC_BugetPrevId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRandValue_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BVC_BugetPrevRandId");

            migrationBuilder.CreateIndex(
                name: "IX_BVC_BugetPrevRand_BVC_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BVC_BugetPrevId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BVC_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BVC_BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BVC_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BVC_BugetPrevRandId",
                principalTable: "BVC_BugetPrevRand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BVC_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BVC_BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_BVC_FormularId",
                table: "BVC_FormRand",
                column: "BVC_FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
