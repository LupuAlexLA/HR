using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateTable_SavedBalanceViewDet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrValueSum",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropColumn(
                name: "CrValueT",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropColumn(
                name: "DbValueSum",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropColumn(
                name: "DbValueT",
                table: "SavedBalanceViewDet");

            migrationBuilder.AddColumn<string>(
                name: "Cont",
                table: "SavedBalanceViewDet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Denumire",
                table: "SavedBalanceViewDet",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cont",
                table: "SavedBalanceViewDet");

            migrationBuilder.DropColumn(
                name: "Denumire",
                table: "SavedBalanceViewDet");

            migrationBuilder.AddColumn<decimal>(
                name: "CrValueSum",
                table: "SavedBalanceViewDet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CrValueT",
                table: "SavedBalanceViewDet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DbValueSum",
                table: "SavedBalanceViewDet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DbValueT",
                table: "SavedBalanceViewDet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
