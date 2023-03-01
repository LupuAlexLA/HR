using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class updatePaapREdistribuireDetaliiV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "BVC_PaapRedistribuireDetalii",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "BVC_PaapRedistribuireDetalii");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "BVC_PaapRedistribuireDetalii");
        }
    }
}
