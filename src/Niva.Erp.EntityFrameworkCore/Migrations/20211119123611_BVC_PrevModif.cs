using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_PrevModif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BugetPrevRandId",
                principalTable: "BVC_BugetPrevRand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BugetPrevRandId",
                principalTable: "BVC_BugetPrevRand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
