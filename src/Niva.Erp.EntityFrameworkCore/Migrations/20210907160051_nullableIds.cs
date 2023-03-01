using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class nullableIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_DobanziReferinta_DobanziReferintaId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_DocumentType_DocumentTypeId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_ImprumutTermene_ImprumuturiTermenId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_ImprumutTipuri_ImprumuturiTipuriId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_Imprumuturi_ImprumutId",
                table: "Rate");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumutId",
                table: "Rate",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentAccountId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LoanAccountId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumuturiTipuriId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumuturiTermenId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DobanziReferintaId",
                table: "Imprumuturi",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumutId",
                table: "Garantie",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_DobanziReferinta_DobanziReferintaId",
                table: "Imprumuturi",
                column: "DobanziReferintaId",
                principalTable: "DobanziReferinta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_DocumentType_DocumentTypeId",
                table: "Imprumuturi",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_ImprumutTermene_ImprumuturiTermenId",
                table: "Imprumuturi",
                column: "ImprumuturiTermenId",
                principalTable: "ImprumutTermene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_ImprumutTipuri_ImprumuturiTipuriId",
                table: "Imprumuturi",
                column: "ImprumuturiTipuriId",
                principalTable: "ImprumutTipuri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_Imprumuturi_ImprumutId",
                table: "Rate",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_DobanziReferinta_DobanziReferintaId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_DocumentType_DocumentTypeId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_ImprumutTermene_ImprumuturiTermenId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Imprumuturi_ImprumutTipuri_ImprumuturiTipuriId",
                table: "Imprumuturi");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_Imprumuturi_ImprumutId",
                table: "Rate");

            migrationBuilder.AlterColumn<int>(
                name: "ImprumutId",
                table: "Rate",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentAccountId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LoanAccountId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ImprumuturiTipuriId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ImprumuturiTermenId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DobanziReferintaId",
                table: "Imprumuturi",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ImprumutId",
                table: "Garantie",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Garantie_Imprumuturi_ImprumutId",
                table: "Garantie",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_DobanziReferinta_DobanziReferintaId",
                table: "Imprumuturi",
                column: "DobanziReferintaId",
                principalTable: "DobanziReferinta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_DocumentType_DocumentTypeId",
                table: "Imprumuturi",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_ImprumutTermene_ImprumuturiTermenId",
                table: "Imprumuturi",
                column: "ImprumuturiTermenId",
                principalTable: "ImprumutTermene",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Imprumuturi_ImprumutTipuri_ImprumuturiTipuriId",
                table: "Imprumuturi",
                column: "ImprumuturiTipuriId",
                principalTable: "ImprumutTipuri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_Imprumuturi_ImprumutId",
                table: "Rate",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
