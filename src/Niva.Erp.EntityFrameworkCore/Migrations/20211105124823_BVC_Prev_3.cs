using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BVC_Prev_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRandDetails_BVC_FormRand_FormRandId",
                table: "BVC_FormRandDetails");

            migrationBuilder.AlterColumn<int>(
                name: "BugetPrevId",
                table: "BVC_BugetPrevStatus",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BugetPrevId",
                table: "BVC_BugetPrevRand",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BugetPrevRandId",
                principalTable: "BVC_BugetPrevRand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRandDetails_BVC_FormRand_FormRandId",
                table: "BVC_FormRandDetails",
                column: "FormRandId",
                principalTable: "BVC_FormRand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand");

            migrationBuilder.DropForeignKey(
                name: "FK_BVC_FormRandDetails_BVC_FormRand_FormRandId",
                table: "BVC_FormRandDetails");

            migrationBuilder.AlterColumn<int>(
                name: "BugetPrevId",
                table: "BVC_BugetPrevStatus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BugetPrevId",
                table: "BVC_BugetPrevRand",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRand_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevRand",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevRandValue_BVC_BugetPrevRand_BugetPrevRandId",
                table: "BVC_BugetPrevRandValue",
                column: "BugetPrevRandId",
                principalTable: "BVC_BugetPrevRand",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_BugetPrevStatus_BVC_BugetPrev_BugetPrevId",
                table: "BVC_BugetPrevStatus",
                column: "BugetPrevId",
                principalTable: "BVC_BugetPrev",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRand_BVC_Formular_FormularId",
                table: "BVC_FormRand",
                column: "FormularId",
                principalTable: "BVC_Formular",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BVC_FormRandDetails_BVC_FormRand_FormRandId",
                table: "BVC_FormRandDetails",
                column: "FormRandId",
                principalTable: "BVC_FormRand",
                principalColumn: "Id");
        }
    }
}
