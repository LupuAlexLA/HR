using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateImoAssetItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProcessedIn",
                table: "ImoAssetItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProcessedInUse",
                table: "ImoAssetItem",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedIn",
                table: "ImoAssetItem");

            migrationBuilder.DropColumn(
                name: "ProcessedInUse",
                table: "ImoAssetItem");
        }
    }
}
