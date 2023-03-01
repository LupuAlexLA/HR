using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddPersonToImoAssetOper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonStoreInId",
                table: "ImoAssetOper",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonStoreOutId",
                table: "ImoAssetOper",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOper_PersonStoreInId",
                table: "ImoAssetOper",
                column: "PersonStoreInId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOper_PersonStoreOutId",
                table: "ImoAssetOper",
                column: "PersonStoreOutId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOper_Persons_PersonStoreInId",
                table: "ImoAssetOper",
                column: "PersonStoreInId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOper_Persons_PersonStoreOutId",
                table: "ImoAssetOper",
                column: "PersonStoreOutId",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOper_Persons_PersonStoreInId",
                table: "ImoAssetOper");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOper_Persons_PersonStoreOutId",
                table: "ImoAssetOper");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOper_PersonStoreInId",
                table: "ImoAssetOper");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOper_PersonStoreOutId",
                table: "ImoAssetOper");

            migrationBuilder.DropColumn(
                name: "PersonStoreInId",
                table: "ImoAssetOper");

            migrationBuilder.DropColumn(
                name: "PersonStoreOutId",
                table: "ImoAssetOper");
        }
    }
}
