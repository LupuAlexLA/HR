using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class formder1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRandDetails_BVC_FormRand_BVC_FormRandId",
                table: "BVC_FormRandDetails");

            migrationBuilder.DropIndex(
                name: "IX_BVC_FormRandDetails_BVC_FormRandId",
                table: "BVC_FormRandDetails");

            migrationBuilder.DropColumn(
                name: "BVC_FormRandId",
                table: "BVC_FormRandDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BVC_FormRandId",
                table: "BVC_FormRandDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_FormRandDetails_BVC_FormRandId",
                table: "BVC_FormRandDetails",
                column: "BVC_FormRandId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRandDetails_BVC_FormRand_BVC_FormRandId",
                table: "BVC_FormRandDetails",
                column: "BVC_FormRandId",
                principalTable: "BVC_FormRand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
