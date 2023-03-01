using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class update_Cupiuri_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationDate",
                table: "CupiuriItem");

            migrationBuilder.AddColumn<int>(
                name: "CupiuriInitId",
                table: "CupiuriItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CupiuriInit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    OperationDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CupiuriInit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CupiuriItem_CupiuriInitId",
                table: "CupiuriItem",
                column: "CupiuriInitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CupiuriItem_CupiuriInit_CupiuriInitId",
                table: "CupiuriItem",
                column: "CupiuriInitId",
                principalTable: "CupiuriInit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CupiuriItem_CupiuriInit_CupiuriInitId",
                table: "CupiuriItem");

            migrationBuilder.DropTable(
                name: "CupiuriInit");

            migrationBuilder.DropIndex(
                name: "IX_CupiuriItem_CupiuriInitId",
                table: "CupiuriItem");

            migrationBuilder.DropColumn(
                name: "CupiuriInitId",
                table: "CupiuriItem");

            migrationBuilder.AddColumn<DateTime>(
                name: "OperationDate",
                table: "CupiuriItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
