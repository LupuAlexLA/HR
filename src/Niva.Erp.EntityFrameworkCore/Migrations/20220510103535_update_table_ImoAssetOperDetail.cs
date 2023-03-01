using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_table_ImoAssetOperDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewAssetAccountId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewAssetAccountInUseId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewDepreciationAccountId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewExpenseAccountId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldAssetAccountId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldAssetAccountInUseId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldDepreciationAccountId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldExpenseAccountId",
                table: "ImoAssetOperDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_NewAssetAccountId",
                table: "ImoAssetOperDetail",
                column: "NewAssetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_NewAssetAccountInUseId",
                table: "ImoAssetOperDetail",
                column: "NewAssetAccountInUseId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_NewDepreciationAccountId",
                table: "ImoAssetOperDetail",
                column: "NewDepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_NewExpenseAccountId",
                table: "ImoAssetOperDetail",
                column: "NewExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_OldAssetAccountId",
                table: "ImoAssetOperDetail",
                column: "OldAssetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_OldAssetAccountInUseId",
                table: "ImoAssetOperDetail",
                column: "OldAssetAccountInUseId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_OldDepreciationAccountId",
                table: "ImoAssetOperDetail",
                column: "OldDepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImoAssetOperDetail_OldExpenseAccountId",
                table: "ImoAssetOperDetail",
                column: "OldExpenseAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewAssetAccountId",
                table: "ImoAssetOperDetail",
                column: "NewAssetAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewAssetAccountInUseId",
                table: "ImoAssetOperDetail",
                column: "NewAssetAccountInUseId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewDepreciationAccountId",
                table: "ImoAssetOperDetail",
                column: "NewDepreciationAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewExpenseAccountId",
                table: "ImoAssetOperDetail",
                column: "NewExpenseAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldAssetAccountId",
                table: "ImoAssetOperDetail",
                column: "OldAssetAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldAssetAccountInUseId",
                table: "ImoAssetOperDetail",
                column: "OldAssetAccountInUseId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldDepreciationAccountId",
                table: "ImoAssetOperDetail",
                column: "OldDepreciationAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldExpenseAccountId",
                table: "ImoAssetOperDetail",
                column: "OldExpenseAccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewAssetAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewAssetAccountInUseId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewDepreciationAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_NewExpenseAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldAssetAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldAssetAccountInUseId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldDepreciationAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImoAssetOperDetail_Account_OldExpenseAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_NewAssetAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_NewAssetAccountInUseId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_NewDepreciationAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_NewExpenseAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_OldAssetAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_OldAssetAccountInUseId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_OldDepreciationAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImoAssetOperDetail_OldExpenseAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "NewAssetAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "NewAssetAccountInUseId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "NewDepreciationAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "NewExpenseAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "OldAssetAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "OldAssetAccountInUseId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "OldDepreciationAccountId",
                table: "ImoAssetOperDetail");

            migrationBuilder.DropColumn(
                name: "OldExpenseAccountId",
                table: "ImoAssetOperDetail");
        }
    }
}
