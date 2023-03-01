using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_ImprumutTipDetaliu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImprumutTipDetalii_Account_ContImprumutId",
                table: "ImprumutTipDetalii");

            migrationBuilder.DropIndex(
                name: "IX_ImprumutTipDetalii_ContImprumutId",
                table: "ImprumutTipDetalii");

            migrationBuilder.DropColumn(
                name: "ContImprumutId",
                table: "ImprumutTipDetalii");

            migrationBuilder.AddColumn<string>(
                name: "ContImprumut",
                table: "ImprumutTipDetalii",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContImprumut",
                table: "ImprumutTipDetalii");

            migrationBuilder.AddColumn<int>(
                name: "ContImprumutId",
                table: "ImprumutTipDetalii",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ImprumutTipDetalii_ContImprumutId",
                table: "ImprumutTipDetalii",
                column: "ContImprumutId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImprumutTipDetalii_Account_ContImprumutId",
                table: "ImprumutTipDetalii",
                column: "ContImprumutId",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}
