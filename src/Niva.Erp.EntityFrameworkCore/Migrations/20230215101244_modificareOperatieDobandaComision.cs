using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class modificareOperatieDobandaComision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RataId",
                table: "OperatieDobandaComision",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaComision",
                table: "OperatieDobandaComision",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaDobanda",
                table: "OperatieDobandaComision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatieDobandaComision_RataId",
                table: "OperatieDobandaComision",
                column: "RataId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperatieDobandaComision_Rate_RataId",
                table: "OperatieDobandaComision",
                column: "RataId",
                principalTable: "Rate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperatieDobandaComision_Rate_RataId",
                table: "OperatieDobandaComision");

            migrationBuilder.DropIndex(
                name: "IX_OperatieDobandaComision_RataId",
                table: "OperatieDobandaComision");

            migrationBuilder.DropColumn(
                name: "RataId",
                table: "OperatieDobandaComision");

            migrationBuilder.DropColumn(
                name: "SumaComision",
                table: "OperatieDobandaComision");

            migrationBuilder.DropColumn(
                name: "SumaDobanda",
                table: "OperatieDobandaComision");
        }
    }
}
