using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class Update_PAAP_Table_V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_Paap_PaapId",
                table: "BVC_PAAP");

            migrationBuilder.DropForeignKey(
                name: "FK_PAAP_Stari_Paap_PAAPId",
                table: "PAAP_Stari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Paap",
                table: "Paap");

            migrationBuilder.RenameTable(
                name: "Paap",
                newName: "PAAP");

            migrationBuilder.RenameColumn(
                name: "PaapId",
                table: "BVC_PAAP",
                newName: "PAAPId");

            migrationBuilder.RenameIndex(
                name: "IX_BVC_PAAP_PaapId",
                table: "BVC_PAAP",
                newName: "IX_BVC_PAAP_PAAPId");

            migrationBuilder.AlterColumn<int>(
                name: "PAAPId",
                table: "BVC_PAAP",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PAAP",
                table: "PAAP",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_PAAP_PAAPId",
                table: "BVC_PAAP",
                column: "PAAPId",
                principalTable: "PAAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PAAP_Stari_PAAP_PAAPId",
                table: "PAAP_Stari",
                column: "PAAPId",
                principalTable: "PAAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_PAAP_PAAP_PAAPId",
                table: "BVC_PAAP");

            migrationBuilder.DropForeignKey(
                name: "FK_PAAP_Stari_PAAP_PAAPId",
                table: "PAAP_Stari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PAAP",
                table: "PAAP");

            migrationBuilder.RenameTable(
                name: "PAAP",
                newName: "Paap");

            migrationBuilder.RenameColumn(
                name: "PAAPId",
                table: "BVC_PAAP",
                newName: "PaapId");

            migrationBuilder.RenameIndex(
                name: "IX_BVC_PAAP_PAAPId",
                table: "BVC_PAAP",
                newName: "IX_BVC_PAAP_PaapId");

            migrationBuilder.AlterColumn<int>(
                name: "PaapId",
                table: "BVC_PAAP",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paap",
                table: "Paap",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_PAAP_Paap_PaapId",
                table: "BVC_PAAP",
                column: "PaapId",
                principalTable: "Paap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PAAP_Stari_Paap_PAAPId",
                table: "PAAP_Stari",
                column: "PAAPId",
                principalTable: "Paap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
