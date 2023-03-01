using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Issuer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Imprumuturi",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_BankId",
                table: "Imprumuturi",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_Issuer_BankId",
                table: "Imprumuturi",
                column: "BankId",
                principalTable: "Issuer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_Issuer_BankId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_Imprumuturi_BankId",
                table: "Imprumuturi");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Imprumuturi");
        }
    }
}
