using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddAssetAccountInUseToImoAssetItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetAccountInUseId",
                table: "ImoAssetItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetItem_AssetAccountInUseId",
                table: "ImoAssetItem",
                column: "AssetAccountInUseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetItem_Account_AssetAccountInUseId",
                table: "ImoAssetItem",
                column: "AssetAccountInUseId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetItem_Account_AssetAccountInUseId",
                table: "ImoAssetItem");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetItem_AssetAccountInUseId",
                table: "ImoAssetItem");

            migrationBuilder.DropColumn(
                name: "AssetAccountInUseId",
                table: "ImoAssetItem");
        }
    }
}
