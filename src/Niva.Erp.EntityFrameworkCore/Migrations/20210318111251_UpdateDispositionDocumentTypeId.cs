using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateDispositionDocumentTypeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_DocumentType_DocumentTypeId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "Dispositions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_DocumentType_DocumentTypeId",
                table: "Dispositions",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_DocumentType_DocumentTypeId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "Dispositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_DocumentType_DocumentTypeId",
                table: "Dispositions",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
