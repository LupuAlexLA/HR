using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Niva.Erp.Migrations
{
    public partial class BvcVenituriV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataReinvestire",
                table: "BVC_VenitTitluCF",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SumaReinvestita",
                table: "BVC_VenitTitluCF",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "TipPlasament",
                table: "BVC_VenitTitlu",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataReinvestire",
                table: "BVC_VenitTitluCF");

            migrationBuilder.DropColumn(
                name: "SumaReinvestita",
                table: "BVC_VenitTitluCF");

            migrationBuilder.AlterColumn<string>(
                name: "TipPlasament",
                table: "BVC_VenitTitlu",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
