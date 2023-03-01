using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DataComisionIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataComision_Comisioane_ComisionId",
                table: "DataComision");

            migrationBuilder.DropForeignKey(
                name: "FK_DataComision_Imprumuturi_ImprumutId",
                table: "DataComision");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DataComision",
                table: "DataComision");

            migrationBuilder.RenameTable(
                name: "DataComision",
                newName: "DateComision");

            migrationBuilder.RenameIndex(
                name: "IX_DataComision_ImprumutId",
                table: "DateComision",
                newName: "IX_DateComision_ImprumutId");

            migrationBuilder.RenameIndex(
                name: "IX_DataComision_ComisionId",
                table: "DateComision",
                newName: "IX_DateComision_ComisionId");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "DateComision",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DateComision",
                table: "DateComision",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DateComision_Comisioane_ComisionId",
                table: "DateComision",
                column: "ComisionId",
                principalTable: "Comisioane",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DateComision_Imprumuturi_ImprumutId",
                table: "DateComision",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateComision_Comisioane_ComisionId",
                table: "DateComision");

            migrationBuilder.DropForeignKey(
                name: "FK_DateComision_Imprumuturi_ImprumutId",
                table: "DateComision");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DateComision",
                table: "DateComision");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "DateComision");

            migrationBuilder.RenameTable(
                name: "DateComision",
                newName: "DataComision");

            migrationBuilder.RenameIndex(
                name: "IX_DateComision_ImprumutId",
                table: "DataComision",
                newName: "IX_DataComision_ImprumutId");

            migrationBuilder.RenameIndex(
                name: "IX_DateComision_ComisionId",
                table: "DataComision",
                newName: "IX_DataComision_ComisionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataComision",
                table: "DataComision",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DataComision_Comisioane_ComisionId",
                table: "DataComision",
                column: "ComisionId",
                principalTable: "Comisioane",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DataComision_Imprumuturi_ImprumutId",
                table: "DataComision",
                column: "ImprumutId",
                principalTable: "Imprumuturi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
