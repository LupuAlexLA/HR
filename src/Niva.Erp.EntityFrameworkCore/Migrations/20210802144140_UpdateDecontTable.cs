using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateDecontTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DiurnaLegala",
                table: "Decont",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiurnaLegala",
                table: "Decont");

            migrationBuilder.AddColumn<int>(
                name: "DiurnaLegalaId",
                table: "Decont",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decont_DiurnaLegalaId",
                table: "Decont",
                column: "DiurnaLegalaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decont_DiurnaLegala_DiurnaLegalaId",
                table: "Decont",
                column: "DiurnaLegalaId",
                principalTable: "DiurnaLegala",
                principalColumn: "Id");
        }
    }
}
