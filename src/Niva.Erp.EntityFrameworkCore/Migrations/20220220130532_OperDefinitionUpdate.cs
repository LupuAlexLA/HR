using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class OperDefinitionUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinition_DocumentType_DocumentTypeId",
                table: "OperationDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_Account_CreditId",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_Account_DebitId",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails");

            migrationBuilder.AlterColumn<int>(
                name: "OperationDefinitionId",
                table: "OperationDefinitionDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DebitId",
                table: "OperationDefinitionDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreditId",
                table: "OperationDefinitionDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperationDefinitionId1",
                table: "OperationDefinitionDetails",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "OperationDefinition",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "OperationDefinition",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OperationDefinitionDetails_OperationDefinitionId1",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationDefinition_CurrencyId",
                table: "OperationDefinition",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinition_Currency_CurrencyId",
                table: "OperationDefinition",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinition_DocumentType_DocumentTypeId",
                table: "OperationDefinition",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_Account_CreditId",
                table: "OperationDefinitionDetails",
                column: "CreditId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_Account_DebitId",
                table: "OperationDefinitionDetails",
                column: "DebitId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId",
                principalTable: "OperationDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId1",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId1",
                principalTable: "OperationDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinition_Currency_CurrencyId",
                table: "OperationDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinition_DocumentType_DocumentTypeId",
                table: "OperationDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_Account_CreditId",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_Account_DebitId",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId1",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationDefinitionDetails_OperationDefinitionId1",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropIndex(
                name: "IX_OperationDefinition_CurrencyId",
                table: "OperationDefinition");

            migrationBuilder.DropColumn(
                name: "OperationDefinitionId1",
                table: "OperationDefinitionDetails");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "OperationDefinition");

            migrationBuilder.AlterColumn<int>(
                name: "OperationDefinitionId",
                table: "OperationDefinitionDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DebitId",
                table: "OperationDefinitionDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CreditId",
                table: "OperationDefinitionDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "OperationDefinition",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinition_DocumentType_DocumentTypeId",
                table: "OperationDefinition",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_Account_CreditId",
                table: "OperationDefinitionDetails",
                column: "CreditId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_Account_DebitId",
                table: "OperationDefinitionDetails",
                column: "DebitId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDefinitionDetails_OperationDefinition_OperationDefinitionId",
                table: "OperationDefinitionDetails",
                column: "OperationDefinitionId",
                principalTable: "OperationDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
