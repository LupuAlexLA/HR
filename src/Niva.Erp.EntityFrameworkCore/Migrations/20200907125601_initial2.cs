using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_Currency_LocalCurrencyId",
                table: "AbpTenants");

            migrationBuilder.AlterColumn<int>(
                name: "LocalCurrencyId",
                table: "AbpTenants",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LegalPersonId",
                table: "AbpTenants",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_Currency_LocalCurrencyId",
                table: "AbpTenants",
                column: "LocalCurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_Currency_LocalCurrencyId",
                table: "AbpTenants");

            migrationBuilder.AlterColumn<int>(
                name: "LocalCurrencyId",
                table: "AbpTenants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LegalPersonId",
                table: "AbpTenants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_Currency_LocalCurrencyId",
                table: "AbpTenants",
                column: "LocalCurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
