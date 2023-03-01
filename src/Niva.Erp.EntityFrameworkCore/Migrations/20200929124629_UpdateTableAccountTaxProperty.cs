using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class UpdateTableAccountTaxProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AccountTaxProperties",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "AccountTaxProperties",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AccountTaxProperties",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "AccountTaxProperties",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AccountTaxProperties");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "AccountTaxProperties");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AccountTaxProperties");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "AccountTaxProperties");
        }
    }
}
