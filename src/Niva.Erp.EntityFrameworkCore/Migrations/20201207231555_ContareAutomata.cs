using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ContareAutomata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountNededId",
                table: "AccountTaxProperties",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyType",
                table: "AccountTaxProperties",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTaxProperties_AccountNededId",
                table: "AccountTaxProperties",
                column: "AccountNededId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTaxProperties_Account_AccountNededId",
                table: "AccountTaxProperties",
                column: "AccountNededId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTaxProperties_Account_AccountNededId",
                table: "AccountTaxProperties");

            migrationBuilder.DropIndex(
                name: "IX_AccountTaxProperties_AccountNededId",
                table: "AccountTaxProperties");

            migrationBuilder.DropColumn(
                name: "AccountNededId",
                table: "AccountTaxProperties");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "AccountTaxProperties");
        }
    }
}
