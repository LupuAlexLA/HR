using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class nullableIdsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateDobanziReferinta_DobanziReferinta_DobanziReferintaId",
                table: "DateDobanziReferinta");

            migrationBuilder.AlterColumn<int>(
                name: "DobanziReferintaId",
                table: "DateDobanziReferinta",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DateDobanziReferinta_DobanziReferinta_DobanziReferintaId",
                table: "DateDobanziReferinta",
                column: "DobanziReferintaId",
                principalTable: "DobanziReferinta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateDobanziReferinta_DobanziReferinta_DobanziReferintaId",
                table: "DateDobanziReferinta");

            migrationBuilder.AlterColumn<int>(
                name: "DobanziReferintaId",
                table: "DateDobanziReferinta",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DateDobanziReferinta_DobanziReferinta_DobanziReferintaId",
                table: "DateDobanziReferinta",
                column: "DobanziReferintaId",
                principalTable: "DobanziReferinta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
