using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateBVC_BugetPrevContributie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevContributie_BankAccount_BankId",
                table: "BVC_BugetPrevContributie");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevContributie_Issuer_BankId",
                table: "BVC_BugetPrevContributie",
                column: "BankId",
                principalTable: "Issuer",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevContributie_Issuer_BankId",
                table: "BVC_BugetPrevContributie");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevContributie_BankAccount_BankId",
                table: "BVC_BugetPrevContributie",
                column: "BankId",
                principalTable: "BankAccount",
                principalColumn: "Id");
        }
    }
}
