using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class assetStockModerniz1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeprecReserve",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "ExpenseReserve",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "Reserve",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "TranzDeprecReserve",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "TranzReserve",
                table: "ImoAssetStockModerniz");

            migrationBuilder.AddColumn<decimal>(
                name: "DeprecModerniz",
                table: "ImoAssetStockModerniz",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpenseModerniz",
                table: "ImoAssetStockModerniz",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Moderniz",
                table: "ImoAssetStockModerniz",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TranzDeprecModerniz",
                table: "ImoAssetStockModerniz",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TranzModerniz",
                table: "ImoAssetStockModerniz",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeprecModerniz",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "ExpenseModerniz",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "Moderniz",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "TranzDeprecModerniz",
                table: "ImoAssetStockModerniz");

            migrationBuilder.DropColumn(
                name: "TranzModerniz",
                table: "ImoAssetStockModerniz");

            migrationBuilder.AddColumn<decimal>(
                name: "DeprecReserve",
                table: "ImoAssetStockModerniz",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpenseReserve",
                table: "ImoAssetStockModerniz",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Reserve",
                table: "ImoAssetStockModerniz",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TranzDeprecReserve",
                table: "ImoAssetStockModerniz",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TranzReserve",
                table: "ImoAssetStockModerniz",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
