using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DiurnaLegalaTodecont : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiurnaLegala",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "DiurnaLegalaId",
                table: "Decont",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiurnaLegalaValue",
                table: "Decont",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Decont_DiurnaLegalaId",
                table: "Decont",
                column: "DiurnaLegalaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decont_DiurnaLegala_DiurnaLegalaId",
                table: "Decont",
                column: "DiurnaLegalaId",
                principalTable: "DiurnaLegala",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decont_DiurnaLegala_DiurnaLegalaId",
                table: "Decont");

            migrationBuilder.DropIndex(
                name: "IX_Decont_DiurnaLegalaId",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "DiurnaLegalaId",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "DiurnaLegalaValue",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "DiurnaLegala",
                table: "Decont",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
