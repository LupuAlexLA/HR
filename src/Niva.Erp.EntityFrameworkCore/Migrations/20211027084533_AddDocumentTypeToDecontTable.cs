using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class AddDocumentTypeToDecontTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "Decont",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decont_DocumentTypeId",
                table: "Decont",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decont_DocumentType_DocumentTypeId",
                table: "Decont",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decont_DocumentType_DocumentTypeId",
                table: "Decont");

            migrationBuilder.DropIndex(
                name: "IX_Decont_DocumentTypeId",
                table: "Decont");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "Decont");
        }
    }
}
