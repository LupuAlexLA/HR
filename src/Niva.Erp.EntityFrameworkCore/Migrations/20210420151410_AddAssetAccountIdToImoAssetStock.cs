using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddAssetAccountIdToImoAssetStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetAccountId",
                table: "ImoAssetStock",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetStock_AssetAccountId",
                table: "ImoAssetStock",
                column: "AssetAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetStock_Account_AssetAccountId",
                table: "ImoAssetStock",
                column: "AssetAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetStock_Account_AssetAccountId",
                table: "ImoAssetStock");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetStock_AssetAccountId",
                table: "ImoAssetStock");

            migrationBuilder.DropColumn(
                name: "AssetAccountId",
                table: "ImoAssetStock");
        }
    }
}
