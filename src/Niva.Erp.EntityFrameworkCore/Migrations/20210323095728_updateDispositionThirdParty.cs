using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updateDispositionThirdParty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Persons_ThirdPartyId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "ThirdPartyId",
                table: "Dispositions",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Persons_ThirdPartyId",
                table: "Dispositions",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Persons_ThirdPartyId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<int>(
                name: "ThirdPartyId",
                table: "Dispositions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Persons_ThirdPartyId",
                table: "Dispositions",
                column: "ThirdPartyId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
