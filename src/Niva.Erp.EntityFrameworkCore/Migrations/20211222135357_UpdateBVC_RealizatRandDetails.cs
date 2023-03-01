using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateBVC_RealizatRandDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "BVC_RealizatRandDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BVC_RealizatRandDetails_CurrencyId",
                table: "BVC_RealizatRandDetails",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_RealizatRandDetails_Currency_CurrencyId",
                table: "BVC_RealizatRandDetails",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_RealizatRandDetails_Currency_CurrencyId",
                table: "BVC_RealizatRandDetails");

            migrationBuilder.DropIndex(
                name: "IX_BVC_RealizatRandDetails_CurrencyId",
                table: "BVC_RealizatRandDetails");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "BVC_RealizatRandDetails");
        }
    }
}
