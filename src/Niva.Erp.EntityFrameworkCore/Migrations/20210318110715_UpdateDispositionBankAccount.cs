using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateDispositionBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_BankAccount_BankAccountId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "BankAccountId",
                table: "Dispositions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_BankAccount_BankAccountId",
                table: "Dispositions",
                column: "BankAccountId",
                principalTable: "BankAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_BankAccount_BankAccountId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "BankAccountId",
                table: "Dispositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_BankAccount_BankAccountId",
                table: "Dispositions",
                column: "BankAccountId",
                principalTable: "BankAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
