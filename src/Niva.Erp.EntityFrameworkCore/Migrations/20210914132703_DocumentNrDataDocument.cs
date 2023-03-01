using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class DocumentNrDataDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "Garantie");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDocument",
                table: "Garantie",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DocumentNr",
                table: "Garantie",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataDocument",
                table: "Garantie");

            migrationBuilder.DropColumn(
                name: "DocumentNr",
                table: "Garantie");

            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "Garantie",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
