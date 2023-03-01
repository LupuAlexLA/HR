using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class ContContabilFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ContContabilId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Imprumuturi_ContContabilId",
                table: "Imprumuturi",
                column: "ContContabilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_Account_ContContabilId",
                table: "Imprumuturi",
                column: "ContContabilId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_Account_ContContabilId",
                table: "Imprumuturi");

            migrationBuilder.DropIndex(
                name: "IX_Imprumuturi_ContContabilId",
                table: "Imprumuturi");

            migrationBuilder.AlterColumn<int>(
                name: "ContContabilId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
