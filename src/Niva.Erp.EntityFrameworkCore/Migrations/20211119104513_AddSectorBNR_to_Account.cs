using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddSectorBNR_to_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectorBnrId",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_SectorBnrId",
                table: "Account",
                column: "SectorBnrId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_BNR_Sector_SectorBnrId",
                table: "Account",
                column: "SectorBnrId",
                principalTable: "BNR_Sector",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_BNR_Sector_SectorBnrId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_SectorBnrId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "SectorBnrId",
                table: "Account");
        }
    }
}
