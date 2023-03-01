using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateDispositionDocumentNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Persons_ThridPartyId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_ThridPartyId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "ThridPartyId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentNumber",
                table: "Dispositions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_ThirdPartyId",
                table: "Dispositions",
                column: "ThirdPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Persons_ThirdPartyId",
                table: "Dispositions",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Persons_ThirdPartyId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_ThirdPartyId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentNumber",
                table: "Dispositions",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThridPartyId",
                table: "Dispositions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_ThridPartyId",
                table: "Dispositions",
                column: "ThridPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Persons_ThridPartyId",
                table: "Dispositions",
                column: "ThridPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
