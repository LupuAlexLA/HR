using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DecontCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Decont",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Decont_CurrencyId",
                table: "Decont",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decont_Currency_CurrencyId",
                table: "Decont",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decont_Currency_CurrencyId",
                table: "Decont");

            migrationBuilder.DropIndex(
                name: "IX_Decont_CurrencyId",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Decont");
        }
    }
}
